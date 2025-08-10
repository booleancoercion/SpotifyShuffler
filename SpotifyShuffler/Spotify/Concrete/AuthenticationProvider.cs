namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

public class AuthenticationProvider : IAuthenticationProvider
{
    private readonly ILogger _logger;
    private readonly AuthenticationConfiguration _configuration;
    private readonly IApiWrapper _apiWrapper;
    private readonly ICsrfStore _csrfStore;

    public AuthenticationProvider(
        ILogger<AuthenticationProvider> logger,
        AuthenticationConfiguration configuration,
        IApiWrapper apiWrapper,
        ICsrfStore csrfStore)
    {
        _logger = logger;
        _configuration = configuration;
        _apiWrapper = apiWrapper;
        _csrfStore = csrfStore;
    }

    public string GetUserAuthorizationUri(string? scope = null, bool showDialog = false)
    {
        QueryBuilder qb = new()
        {
            {"client_id", _configuration.ClientId},
            {"response_type", "code"},
            {"redirect_uri", _configuration.RedirectUri},
            {"show_dialog", showDialog.ToString().ToLower()},
            {"state", _csrfStore.Generate()}
        };

        if (!string.IsNullOrEmpty(scope))
        {
            qb.Add("scope", scope);
        }

        return $"{_configuration.AuthorizeUri}{qb}";
    }
}
