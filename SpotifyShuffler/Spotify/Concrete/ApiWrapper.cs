namespace booleancoercion.SpotifyShuffler.Spotify;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using booleancoercion.SpotifyShuffler.Util;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public class ApiWrapper : IApiWrapper
{
    private readonly ILogger _logger;
    private readonly ApiConfiguration _configuration;

    public ApiWrapper(ILogger<ApiWrapper> logger, ApiConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<(string Token, TimeSpan ExpiresIn, string RefreshToken)> GetToken(
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

        try
        {
            JsonElement json = await SendRequestAndDeserialize(request, cancellationToken);

            string token = json.GetProperty("access_token").GetString() ?? throw new Exception("Access token is null");
            string refreshToken = json.GetProperty("refresh_token").GetString() ?? throw new Exception("Refresh token is null");
            int expiresInSeconds = json.GetProperty("expires_in").GetInt32();

            return (token, TimeSpan.FromSeconds(expiresInSeconds), refreshToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, traceId, "Error when querying and deserializing response");
            throw;
        }
    }

    public async Task<(string Id, string? DisplayName)> GetCurrentUserProfile(TraceId traceId, string token, CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, _configuration.BaseApiUri + _configuration.MeEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            JsonElement json = await SendRequestAndDeserialize(request, cancellationToken);

            string id = json.GetProperty("id").GetString() ?? throw new Exception("ID is null");
            string? displayName = json.GetProperty("display_name").GetString();

            return (id, displayName);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, traceId, "Error when querying and deserializing response");
            throw;
        }
    }

    private static async Task<JsonElement> SendRequestAndDeserialize(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = new();
        HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        JsonElement? json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);

        if (json is not JsonElement jsonElement)
        {
            string raw = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Could not deserialize response from api: {raw}");
        }

        return jsonElement;
    }
}
