namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using booleancoercion.SpotifyShuffler.Spotify.Entities;
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

    public string GetUserAuthorizationUri(bool showDialog = false)
    {
        QueryBuilder qb = new()
        {
            {"client_id", _configuration.ClientId},
            {"response_type", "code"},
            {"redirect_uri", _configuration.RedirectUri},
            {"show_dialog", showDialog.ToString().ToLower()},
            {"state", _csrfStore.Generate()},
            {"scope", _configuration.Scopes},
        };

        return $"{_configuration.AuthorizeUri}{qb}";
    }

    public async Task<bool> TryRegisterUserAsync(TraceId traceId, string code, string csrfToken, CancellationToken cancellationToken = default)
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
            IApiWrapper.TokenObject tokenObject = await _apiWrapper.GetTokenAsync(
                traceId,
                code,
                _configuration.RedirectUri,
                _configuration.ClientId,
                _configuration.ClientSecret,
                cancellationToken);

            _logger.LogInformation(traceId, "Successfully retrieved token, getting user details");
            User user = await _apiWrapper.GetCurrentUserProfileAsync(traceId, tokenObject.Token, cancellationToken);

            AppUser appUser = new()
            {
                Id = user.Id,
                CachedName = user.DisplayName,
                Token = tokenObject.Token,
                TokenExpiry = now + tokenObject.ExpiresIn,
                RefreshToken = tokenObject.RefreshToken!,
                Scopes = _configuration.Scopes,
            };
            _userStore.UpdateUser(appUser);
            _logger.LogInformation(traceId, $"Received profile details for {appUser}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, traceId, "Error when registering user");
            return false;
        }
    }

    public async Task<string> GetTokenAsync(TraceId traceId, string userId, TimeSpan? refreshIfExpiresIn = null, CancellationToken cancellationToken = default)
    {
        refreshIfExpiresIn ??= TimeSpan.FromMinutes(1);

        AppUser user = _userStore.GetUser(userId);
        if (user.Scopes != _configuration.Scopes)
        {
            _userStore.RemoveUser(userId);
            throw new Exception($"Required scope configuration changed since token for user {user} was acquired!"
            + " Cannot use stored token, and refreshing will not help. User removed from store.");
        }
        else if (user.TokenExpiry - _timeProvider.Now < refreshIfExpiresIn)
        {
            _logger.LogInformation(traceId, $"Expiry threshold reached, refreshing token for user {user}");
        }
        else
        {
            return user.Token;
        }

        try
        {
            DateTime now = _timeProvider.Now;
            IApiWrapper.TokenObject tokenObject = await _apiWrapper.RefreshTokenAsync(
                traceId,
                user.RefreshToken,
                _configuration.ClientId,
                _configuration.ClientSecret,
                cancellationToken);

            _logger.LogInformation(traceId, $"Successfully retrieved new token for {user}");
            user.Token = tokenObject.Token;
            user.TokenExpiry = now + tokenObject.ExpiresIn;
            user.RefreshToken = tokenObject.RefreshToken ?? user.RefreshToken;

            return tokenObject.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, traceId, $"Exception while trying to retrieve new token for user {user}");
            throw;
        }
    }
}
