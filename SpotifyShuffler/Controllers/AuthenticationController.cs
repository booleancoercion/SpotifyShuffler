namespace booleancoercion.SpotifyShuffler.Controllers;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Concrete;
using booleancoercion.SpotifyShuffler.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IUserStore _userStore;
    private readonly IShuffler _shuffler;

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IAuthenticationProvider authenticationProvider,
        IUserStore userStore,
        IShuffler shuffler)
    {
        _logger = logger;
        _authenticationProvider = authenticationProvider;
        _userStore = userStore;
        _shuffler = shuffler;
    }

    [HttpGet]
    [Route("/register")]
    public IActionResult RegisterAsync()
    {
        return Redirect(_authenticationProvider.GetUserAuthorizationUri());
    }

    [HttpGet]
    [Route("/callback")]
    public async Task<IActionResult> OAuthCallbackAsync(
        [FromQuery] string? code,
        [FromQuery] string? error,
        [FromQuery] string state,
        CancellationToken cancellationToken)
    {
        if (code is not null && await _authenticationProvider.TryRegisterUserAsync(new TraceId(), code, state, cancellationToken))
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("/trigger")]
    public IActionResult Trigger(CancellationToken cancellationToken)
    {
        Task _ = Task.Run(async () =>
        {
            TraceId traceId = new();

            foreach (AppUser user in _userStore.GetAllUsers())
            {
                await _shuffler.ShuffleForUserAsync(traceId, user);
            }
        });

        return Ok();
    }
}
