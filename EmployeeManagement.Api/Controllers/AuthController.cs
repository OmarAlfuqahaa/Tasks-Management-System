using EmployeeManagement.Application.Features.Auth.Commands.Login;
using EmployeeManagement.Application.Features.Auth.Commands.Logout;
using EmployeeManagement.Application.Features.Auth.Commands.RefreshToken;
using EmployeeManagement.Application.Features.Auth.Commands.Register;
using EmployeeManagement.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.Name, request.Email, request.Password, request.Role);
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _mediator.Send(new LoginCommand(request.Email, request.Password));
        return Ok(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
    {
        var response = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken));
        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        await _mediator.Send(new LogoutCommand(request.RefreshToken));
        return NoContent();
    }
}
