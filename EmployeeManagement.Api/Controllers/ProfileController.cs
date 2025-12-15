using EmployeeManagement.Application.Features.Profile.Commands.ChangePassword;
using EmployeeManagement.Application.Features.Profile.Commands.UpdateProfile;
using EmployeeManagement.Application.Features.Profile.DTOs;
using EmployeeManagement.Application.Features.Profile.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<ProfileResponse>> GetProfile()
    {
        var profile = await _mediator.Send(new GetProfileQuery());
        return Ok(profile);
    }

    [HttpPut]
    public async Task<ActionResult<ProfileResponse>> UpdateProfile(UpdateProfileRequest request)
    {
        var command = new UpdateProfileCommand(request.Name, request.Email);
        var profile = await _mediator.Send(command);
        return Ok(profile);
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var command = new ChangePasswordCommand(request.CurrentPassword, request.NewPassword);
        await _mediator.Send(command);
        return NoContent();
    }
}
