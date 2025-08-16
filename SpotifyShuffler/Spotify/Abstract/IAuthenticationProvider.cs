namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

using booleancoercion.SpotifyShuffler.Util;

public interface IAuthenticationProvider
{
    string GetUserAuthorizationUri(bool showDialog = false);

    Task<bool> TryRegisterUserAsync(TraceId traceId, string code, string csrfToken, CancellationToken cancellationToken = default);

    Task<string> GetTokenAsync(TraceId traceId, string userId, TimeSpan? refreshIfExpiresIn = null, CancellationToken cancellationToken = default);
}
