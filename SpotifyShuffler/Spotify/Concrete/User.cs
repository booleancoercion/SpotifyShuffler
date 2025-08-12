namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

public class User
{
    public required string Id { get; init; }
    public required string? CachedName { get; set; }
    public required string Token { get; set; }
    public required DateTime TokenExpiry { get; set; }
    public required string RefreshToken { get; init; }
}
