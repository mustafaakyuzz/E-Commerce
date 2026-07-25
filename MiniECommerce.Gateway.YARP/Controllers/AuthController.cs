using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Gateway.YARP.Dtos;
using MiniECommerce.Gateway.YARP.Services;

namespace MiniECommerce.Gateway.YARP.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        if (!result.IsSuccessful)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(loginDto, cancellationToken);
        if (!result.IsSuccessful)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
