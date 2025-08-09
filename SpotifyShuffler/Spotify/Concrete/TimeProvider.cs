namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;

public class TimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
}
