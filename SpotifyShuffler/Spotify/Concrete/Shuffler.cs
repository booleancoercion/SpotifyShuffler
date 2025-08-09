namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using Microsoft.Extensions.Logging;

public class Shuffler : IShuffler
{
    private readonly ILogger _logger;
    private readonly IAuthenticationProvider _authenticationProvider;

    public Shuffler(ILogger<Shuffler> logger, IAuthenticationProvider authenticationProvider)
    {
        _logger = logger;
        _authenticationProvider = authenticationProvider;
    }

    public async Task PerformShuffle(string playlistId)
    {
        _logger.LogInformation($"Shuffling playlist with ID {playlistId}");
    }
}
