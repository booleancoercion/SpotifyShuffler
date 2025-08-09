namespace booleancoercion.SpotifyShuffler;

using booleancoercion.SpotifyShuffler.Spotify;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SpotifyShufflerBackgroundService : IHostedService
{
    private readonly ILogger _logger;
    private readonly Shuffler _shuffler;

    public SpotifyShufflerBackgroundService(Shuffler shuffler, ILogger<SpotifyShufflerBackgroundService> logger)
    {
        _logger = logger;
        _shuffler = shuffler;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Service start!");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("Service stop!");
        return Task.CompletedTask;
    }
}
