using EmployeeManagement.Application.Common.Models;
using EmployeeManagement.Application.Features.Users.Commands.DeleteUser;
using EmployeeManagement.Application.Features.Users.Commands.UpdateUser;
using EmployeeManagement.Application.Features.Users.DTOs;
using EmployeeManagement.Application.Features.Users.Queries.GetUserById;
using EmployeeManagement.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

  [HttpGet("employees")]
  [Authorize(Roles = "admin,project-manager, employee")]
  public async Task<ActionResult<List<UserDto>>> GetEmployees()
  {
    var query = new GetUsersQuery
    {
      Page = 1,
      PageSize = 1000,
      Role = "employee"
    };

    var result = await _mediator.Send(query);

    return Ok(result.Items);
  }



  [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
    {
        var updated = await _mediator.Send(new UpdateUserCommand(id, request.Name, request.Email, request.Role));
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _mediator.Send(new DeleteUserCommand(id));
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }



}
