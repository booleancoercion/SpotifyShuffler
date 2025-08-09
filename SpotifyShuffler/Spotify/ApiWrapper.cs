namespace booleancoercion.SpotifyShuffler.Spotify;

using Microsoft.Extensions.Logging;

public class ApiWrapper
{
    private readonly ILogger _logger;
    private readonly ApiConfiguration _configuration;

    public ApiWrapper(ApiConfiguration configuration, ILogger<ApiWrapper> logger)
    {
        _logger = logger;
        _configuration = configuration;
    }
}
