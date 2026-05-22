using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Products.Api.Auth.Interfaces;
using Products.Api.Auth.Login;
using Products.Api.Filters;

namespace Products.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ILoginService _loginService;

    public AuthController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("login")]
    [EnableRateLimiting("Login")]
    [ServiceFilter(typeof(ValidationFilter<LoginRequest>))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _loginService.LoginAsync(request, cancellationToken);

        if (response is null)
        {
            return Unauthorized();
        }

        return Ok(response);
    }
}