namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Util;
using System.Collections.Concurrent;
using System.Security.Cryptography;

public class CsrfStore : ICsrfStore
{
    private readonly ConcurrentDictionary<string, DateTime> _inner = [];
    private readonly ITimeProvider _timeProvider;

    public CsrfStore(ITimeProvider? timeProvider = null)
    {
        _timeProvider = timeProvider ?? new TimeProvider();
    }

    public string Generate()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(128);

        string token = Convert.ToBase64String(bytes);
        _inner.TryAdd(token, _timeProvider.Now); // it's virtually impossible that we'll get a duplicate

        return token;
    }

    public bool Consume(string token)
    {
        return _inner.TryRemove(token, out _);
    }

    public void Cleanup(TimeSpan maxAge)
    {
        ICollection<string> keys = _inner.Keys;
        DateTime now = _timeProvider.Now;
        foreach (string key in keys)
        {
            if (now - _inner[key] > maxAge)
            {
                _inner.Remove(key, out _);
            }
        }
    }
}
