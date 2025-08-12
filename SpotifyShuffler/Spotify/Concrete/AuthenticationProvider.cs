namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using booleancoercion.SpotifyShuffler.Util;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

public class AuthenticationProvider : IAuthenticationProvider
{
    private readonly ILogger _logger;
    private readonly AuthenticationConfiguration _configuration;
    private readonly IApiWrapper _apiWrapper;
    private readonly ICsrfStore _csrfStore;
    private readonly IUserStore _userStore;
    private readonly ITimeProvider _timeProvider;

    public AuthenticationProvider(
        ILogger<AuthenticationProvider> logger,
        AuthenticationConfiguration configuration,
        IApiWrapper apiWrapper,
        ICsrfStore csrfStore,
        IUserStore userStore,
        ITimeProvider? timeProvider = null)
    {
        _logger = logger;
        _configuration = configuration;
        _apiWrapper = apiWrapper;
        _csrfStore = csrfStore;
        _userStore = userStore;
        _timeProvider = timeProvider ?? new TimeProvider();
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

    public async Task<bool> TryRegisterUserAsync(TraceId traceId, string code, string csrfToken, CancellationToken cancellationToken)
    {
        _logger.LogInformation(traceId, "Registering user");
        if (!_csrfStore.Consume(csrfToken))
        {
            _logger.LogInformation(traceId, "Received invalid CSRF token");
            return false;
        }

        try
        {
            _logger.LogInformation(traceId, "CSRF token is valid, proceeding");
            DateTime now = _timeProvider.Now;
            (string token, TimeSpan expiresIn, string refreshToken) = await _apiWrapper.GetToken(
                traceId,
                code,
                _configuration.RedirectUri,
                _configuration.ClientId,
                _configuration.ClientSecret,
                cancellationToken);

            _logger.LogInformation(traceId, "Successfully retrieved token, getting user details");
            (string id, string? displayName) = await _apiWrapper.GetCurrentUserProfile(traceId, token, cancellationToken);

            _logger.LogInformation(traceId, $"Received profile details for {id}" + ((displayName is not null) ? $" ({displayName})" : ""));

            User user = new()
            {
                Id = id,
                CachedName = displayName,
                Token = token,
                TokenExpiry = now + expiresIn,
                RefreshToken = refreshToken,
            };
            _userStore.UpdateUser(user);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, traceId, "Error when registering user");
            return false;
        }
    }
}
