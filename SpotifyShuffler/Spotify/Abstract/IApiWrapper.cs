namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

using booleancoercion.SpotifyShuffler.Util;

public interface IApiWrapper
{
    Task<(string Token, TimeSpan ExpiresIn, string RefreshToken)> GetToken(
        TraceId traceId,
        string code,
        string redirectUri,
        string clientId,
        string clientSecret,
        CancellationToken cancellationToken = default);

    Task<(string Id, string? DisplayName)> GetCurrentUserProfile(TraceId traceId, string token, CancellationToken cancellationToken = default);
}
