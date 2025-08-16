namespace booleancoercion.SpotifyShuffler.Spotify;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Concrete;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using booleancoercion.SpotifyShuffler.Spotify.Entities;
using booleancoercion.SpotifyShuffler.Util;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public class ApiWrapper : IApiWrapper
{
    private readonly ILogger _logger;
    private readonly IRateLimiter _rateLimiter;
    private readonly ApiConfiguration _configuration;

    public ApiWrapper(ILogger<ApiWrapper> logger, ApiConfiguration configuration, IRateLimiter? rateLimiter = null)
    {
        _logger = logger;
        _configuration = configuration;
        _rateLimiter = rateLimiter ?? new RateLimiter(TimeSpan.FromSeconds(0.5));
    }

    public async Task<IApiWrapper.TokenObject> GetTokenAsync(
        TraceId traceId,
        string code,
        string redirectUri,
        string clientId,
        string clientSecret,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, _configuration.TokenUri);
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))
        );
        request.Content = new FormUrlEncodedContent(
        [
            new("grant_type", "authorization_code"),
            new("code", code),
            new("redirect_uri", redirectUri),
        ]);

        TokenObjectRaw tokenObjectRaw = await SendRequestAndDeserializeAsync<TokenObjectRaw>(traceId, request, cancellationToken);
        return tokenObjectRaw.ToTokenObject();
    }

    public async Task<IApiWrapper.TokenObject> RefreshTokenAsync(
        TraceId traceId,
        string refreshToken,
        string clientId,
        string clientSecret,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, _configuration.TokenUri);
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))
        );
        request.Content = new FormUrlEncodedContent(
        [
            new("grant_type", "refresh_token"),
            new("refresh_token", refreshToken),
        ]);

        TokenObjectRaw tokenObjectRaw = await SendRequestAndDeserializeAsync<TokenObjectRaw>(traceId, request, cancellationToken);
        return tokenObjectRaw.ToTokenObject();
    }

    public async Task<User> GetCurrentUserProfileAsync(TraceId traceId, string token, CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, _configuration.BaseApiUri + _configuration.MeEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await SendRequestAndDeserializeAsync<User>(traceId, request, cancellationToken);
    }

    public async Task<IReadOnlyList<Playlist>> GetCurrentUserPlaylistsAsync(TraceId traceId, string token, CancellationToken cancellationToken = default)
    {
        string uri = $"{_configuration.BaseApiUri}{_configuration.MyPlaylistsEndpoint}?limit=50&offset=0";
        List<Playlist> playlists = [];

        while (true)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PlaylistsResponseRaw response = await SendRequestAndDeserializeAsync<PlaylistsResponseRaw>(traceId, request, cancellationToken);
            playlists.AddRange(response.Items);
            if (response.Next is null)
            {
                break;
            }

            uri = response.Next;
        }

        return playlists.AsReadOnly();
    }

    public async Task<IReadOnlyList<TrackOrEpisode>> GetPlaylistTracksAsync(
        TraceId traceId,
        string token,
        string playlistId,
        CancellationToken cancellationToken = default)
    {
        string uri = string.Format($"{_configuration.BaseApiUri}{_configuration.PlaylistTracksEndpoint}", playlistId) + new QueryBuilder()
        {
            {"fields", "next,items(track(id,name,uri,type))"},
            {"limit", "50"}
        };
        List<TrackOrEpisode> tracks = [];

        while (true)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            TracksReponseRaw response = await SendRequestAndDeserializeAsync<TracksReponseRaw>(traceId, request, cancellationToken);
            tracks.AddRange(response.Items.Select(item => item.Track));
            if (response.Next is null)
            {
                break;
            }

            uri = response.Next;
        }

        return tracks.AsReadOnly();
    }

    public async Task<string> SetPlaylistTracksAsync(
        TraceId traceId,
        string token,
        string playlistId,
        IEnumerable<string> uris,
        CancellationToken cancellationToken = default)
    {
        return await SetAddPlaylistTracksInnerAsync(HttpMethod.Put, traceId, token, playlistId, uris, cancellationToken);
    }

    public async Task<string> AddPlaylistTracksAsync(
        TraceId traceId,
        string token,
        string playlistId,
        IEnumerable<string> uris,
        CancellationToken cancellationToken = default)
    {
        return await SetAddPlaylistTracksInnerAsync(HttpMethod.Post, traceId, token, playlistId, uris, cancellationToken);
    }

    private async Task<string> SetAddPlaylistTracksInnerAsync(
        HttpMethod httpMethod,
        TraceId traceId,
        string token,
        string playlistId,
        IEnumerable<string> uris,
        CancellationToken cancellationToken = default)
    {
        string[] uriArray = uris.ToArray();
        if (uriArray.Length > 100)
        {
            throw new Exception($"The number of URIs is {uriArray.Length}, which is more than the maximum of 100.");
        }

        string uri = string.Format($"{_configuration.BaseApiUri}{_configuration.PlaylistTracksEndpoint}", playlistId);
        using HttpRequestMessage request = new(httpMethod, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new Dictionary<string, object>() { { "uris", uriArray } });

        JsonElement response = await SendRequestAndDeserializeAsync<JsonElement>(traceId, request, cancellationToken);
        return response.GetProperty("snapshot_id").GetString() ?? throw new Exception("Snapshot ID is null");
    }

    private async Task<T> SendRequestAndDeserializeAsync<T>(TraceId traceId, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(traceId, $"Sending request: {request.Method} {request.RequestUri}");

        await _rateLimiter.WaitAsync(cancellationToken);
        try
        {
            using HttpClient httpClient = new();
            HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);

            long? contentLength = response.Content.Headers.ContentLength;
            _logger.LogInformation(traceId, $"Received response: {response.StatusCode}{(contentLength.HasValue ? $" ({contentLength} bytes)" : "")}");

            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };
            T? json = await response.Content.ReadFromJsonAsync<T>(options, cancellationToken);

            if (json is not T deserialized)
            {
                string raw = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Could not deserialize response from api: {raw}");
            }

            return deserialized;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, traceId, "Error when querying and deserializing response");
            throw;
        }
        finally
        {
            _rateLimiter.Release();
        }
    }

    private class TokenObjectRaw
    {
        public required string AccessToken { get; set; }
        public required int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }

        public IApiWrapper.TokenObject ToTokenObject()
        {
            return new IApiWrapper.TokenObject()
            {
                Token = AccessToken,
                ExpiresIn = TimeSpan.FromSeconds(ExpiresIn),
                RefreshToken = RefreshToken,
            };
        }
    }

    private class PlaylistsResponseRaw
    {
        public string? Next { get; set; }
        public required Playlist[] Items { get; set; }
    }

    private class PlaylistItem
    {
        public required TrackOrEpisode Track { get; set; }
    }

    private class TracksReponseRaw
    {
        public string? Next { get; set; }
        public required PlaylistItem[] Items { get; set; }
    }
}
