using MediatR;
using Microsoft.AspNetCore.Mvc;
using CoreBanking.Application.Auth.Commands.Register;
using CoreBanking.Application.Auth.Commands.Login;
using CoreBanking.Application.Auth.Commands.RefreshToken;
using CoreBanking.Api.Models;

namespace CoreBanking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand
        {
            Email = request.Email,
            Password = request.Password,
            FullName = request.FullName
        };

        var result = await _mediator.Send(command,ct);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
