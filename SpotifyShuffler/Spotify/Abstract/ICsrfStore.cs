namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

public interface ICsrfStore
{
    string Generate();

    bool Consume(string token);

    void Cleanup(TimeSpan maxAge);
}
