namespace booleancoercion.SpotifyShuffler;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SpotifyShufflerBackgroundService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IUserStore _userStore;

    public SpotifyShufflerBackgroundService(ILogger<SpotifyShufflerBackgroundService> logger, IUserStore userStore)
    {
        _logger = logger;
        _userStore = userStore;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _userStore.InitializeAsync(cancellationToken);

        _logger.LogInformation("Service start!");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _userStore.PersistAsync(cancellationToken);

        _logger.LogWarning("Service stop!");
    }
}
