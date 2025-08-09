namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

public interface IAuthenticationProvider
{
    string GetUserAuthorizationUri(string? scope = null, bool showDialog = false);
}
