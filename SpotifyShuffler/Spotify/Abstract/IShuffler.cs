namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

public interface IShuffler
{
    Task PerformShuffleAsync(string playlistId);
}
