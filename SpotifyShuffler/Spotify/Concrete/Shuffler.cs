namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using Microsoft.Extensions.Logging;

public class Shuffler : IShuffler
{
    private readonly ILogger _logger;

    public Shuffler(ILogger<Shuffler> logger)
    {
        _logger = logger;
    }

    public Task PerformShuffle(string playlistId)
    {
        _logger.LogInformation($"Shuffling playlist with ID {playlistId}");

        return Task.CompletedTask;
    }
}
