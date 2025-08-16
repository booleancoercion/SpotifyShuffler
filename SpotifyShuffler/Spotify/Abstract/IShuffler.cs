namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

using booleancoercion.SpotifyShuffler.Spotify.Concrete;
using booleancoercion.SpotifyShuffler.Util;

public interface IShuffler
{
    Task ShuffleForUserAsync(TraceId traceId, AppUser user, CancellationToken cancellationToken = default);
}
