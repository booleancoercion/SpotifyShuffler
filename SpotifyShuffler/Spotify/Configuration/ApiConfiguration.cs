namespace booleancoercion.SpotifyShuffler.Spotify.Configuration;

public class ApiConfiguration
{
    public required string TokenUri { get; init; }
    public required string BaseApiUri { get; init; }
    public required string MeEndpoint { get; init; }
    public required string MyPlaylistsEndpoint { get; init; }
    public required string PlaylistTracksEndpoint { get; init; }
}
