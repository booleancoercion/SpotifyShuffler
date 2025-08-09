namespace booleancoercion.SpotifyShuffler.Spotify;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using Microsoft.Extensions.Logging;

public class ApiWrapper : IApiWrapper
{
    private readonly ILogger _logger;
    private readonly ApiConfiguration _configuration;

    public ApiWrapper(ILogger<ApiWrapper> logger, ApiConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
}
