namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Util;

public class RateLimiter : IRateLimiter
{
    private readonly TimeSpan _timeBetweenInvocations;
    private readonly ITimeProvider _timeProvider;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private DateTime? _lastRelease = null;

    public RateLimiter(TimeSpan timeBetweenInvocations, ITimeProvider? timeProvider = null)
    {
        _timeBetweenInvocations = timeBetweenInvocations;
        _timeProvider = timeProvider ?? new TimeProvider();
    }

    public async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        if (_lastRelease is null)
        {
            return;
        }

        TimeSpan timeSinceLastInvocation = _timeProvider.Now - _lastRelease.Value;
        if (timeSinceLastInvocation < _timeBetweenInvocations)
        {
            await Task.Delay(_timeBetweenInvocations - timeSinceLastInvocation, cancellationToken);
        }
    }

    public void Release()
    {
        _lastRelease = _timeProvider.Now;
        _semaphore.Release();
    }
}
