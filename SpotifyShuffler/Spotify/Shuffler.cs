namespace booleancoercion.SpotifyShuffler.Spotify;

using Microsoft.Extensions.Logging;

public class Shuffler
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
