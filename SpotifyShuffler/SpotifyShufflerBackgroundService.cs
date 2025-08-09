namespace booleancoercion.SpotifyShuffler;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SpotifyShufflerBackgroundService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IShuffler _shuffler;

    public SpotifyShufflerBackgroundService(IShuffler shuffler, ILogger<SpotifyShufflerBackgroundService> logger)
    {
        _logger = logger;
        _shuffler = shuffler;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Service start!");

        Task.Run(() => _shuffler.PerformShuffle("playlistId"), cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("Service stop!");
        return Task.CompletedTask;
    }
}
