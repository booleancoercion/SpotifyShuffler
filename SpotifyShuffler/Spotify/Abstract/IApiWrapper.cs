namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

using booleancoercion.SpotifyShuffler.Spotify.Entities;
using booleancoercion.SpotifyShuffler.Util;

public interface IApiWrapper
{
    Task<TokenObject> GetTokenAsync(
        TraceId traceId,
        string code,
        string redirectUri,
        string clientId,
        string clientSecret,
        CancellationToken cancellationToken = default);

    Task<TokenObject> RefreshTokenAsync(
        TraceId traceId,
        string refreshToken,
        string clientId,
        string clientSecret,
        CancellationToken cancellationToken = default);

    Task<User> GetCurrentUserProfileAsync(TraceId traceId, string token, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Playlist>> GetCurrentUserPlaylistsAsync(TraceId traceId, string token, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TrackOrEpisode>> GetPlaylistTracksAsync(
        TraceId traceId,
        string token,
        string playlistId,
        CancellationToken cancellationToken = default);

    Task<string> SetPlaylistTracksAsync(
        TraceId traceId,
        string token,
        string playlistId,
        IEnumerable<string> uris,
        CancellationToken cancellationToken = default);

    Task<string> AddPlaylistTracksAsync(
        TraceId traceId,
        string token,
        string playlistId,
        IEnumerable<string> uris,
        CancellationToken cancellationToken = default);

    public class TokenObject
    {
        public required string Token { get; set; }
        public required TimeSpan ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
    }
}
