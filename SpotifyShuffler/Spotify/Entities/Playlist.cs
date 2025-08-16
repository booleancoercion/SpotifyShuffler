namespace booleancoercion.SpotifyShuffler.Spotify.Entities;

public class Playlist
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required User Owner { get; set; }
    public required string SnapshotId { get; set; }

    public override string ToString()
    {
        return $"{Id} ({Name})";
    }
}
