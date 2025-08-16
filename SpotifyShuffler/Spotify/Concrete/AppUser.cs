namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

public class AppUser
{
    public required string Id { get; init; }
    public required string? CachedName { get; set; }
    public required string Token { get; set; }
    public required DateTime TokenExpiry { get; set; }
    public required string RefreshToken { get; set; }
    public required string Scopes { get; set; }

    public override string ToString()
    {
        string nameSuffix = CachedName is null ? "" : $" ({CachedName})";
        return $"{Id}{nameSuffix}";
    }
}
