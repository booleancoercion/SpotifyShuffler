namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

using booleancoercion.SpotifyShuffler.Util;

public interface IAuthenticationProvider
{
    string GetUserAuthorizationUri(string? scope = null, bool showDialog = false);

    Task<bool> TryRegisterUserAsync(TraceId traceId, string code, string csrfToken, CancellationToken cancellationToken);
}
