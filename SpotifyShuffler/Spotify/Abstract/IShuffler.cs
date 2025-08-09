namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

public interface IShuffler
{
    Task PerformShuffle(string playlistId);
}
