namespace booleancoercion.SpotifyShuffler.Controllers;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
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
    public IActionResult OAuthCallback()
    {
        return Ok();
    }
}
