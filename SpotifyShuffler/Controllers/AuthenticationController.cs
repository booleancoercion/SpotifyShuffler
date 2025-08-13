namespace booleancoercion.SpotifyShuffler.Controllers;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IAuthenticationProvider _authenticationProvider;

    public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationProvider authenticationProvider)
    {
        _logger = logger;
        _authenticationProvider = authenticationProvider;
    }

    [HttpGet]
    [Route("/register")]
    public IActionResult Register()
    {
        return Redirect(_authenticationProvider.GetUserAuthorizationUri());
    }

    [HttpGet]
    [Route("/callback")]
    public async Task<IActionResult> OAuthCallback(
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
}
