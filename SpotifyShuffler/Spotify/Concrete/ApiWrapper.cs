namespace booleancoercion.SpotifyShuffler.Spotify;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using Microsoft.Extensions.Logging;

public class ApiWrapper : IApiWrapper
{
    private readonly ILogger _logger;
    private readonly ApiConfiguration _configuration;

    public ApiWrapper(ApiConfiguration configuration, ILogger<ApiWrapper> logger)
    {
        _logger = logger;
        _configuration = configuration;
    }
}
