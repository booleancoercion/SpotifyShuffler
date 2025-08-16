namespace booleancoercion.SpotifyShuffler.Spotify.Configuration;

public class AuthenticationConfiguration
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string RedirectUri { get; init; }
    public required string AuthorizeUri { get; init; }
    public required string Scopes { get; init; }
}
