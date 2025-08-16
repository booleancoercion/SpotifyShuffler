namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

public interface IRateLimiter
{
    Task WaitAsync(CancellationToken cancellationToken = default);
    void Release();
}
