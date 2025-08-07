namespace booleancoercion.SpotifyShuffler;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SpotifyShufflerBackgroundService : IHostedService
{
    private readonly ILogger _logger;

    public SpotifyShufflerBackgroundService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(nameof(SpotifyShufflerBackgroundService));
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
