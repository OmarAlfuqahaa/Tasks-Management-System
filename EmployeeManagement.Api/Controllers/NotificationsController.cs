using EmployeeManagement.Application.Common.Models;
using EmployeeManagement.Application.Features.Notifications.Commands.CreateNotification;
using EmployeeManagement.Application.Features.Notifications.Commands.MarkNotificationsAsRead;
using EmployeeManagement.Application.Features.Notifications.DTOs;
using EmployeeManagement.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<PagedResult<NotificationDto>>> GetNotifications([FromQuery] GetNotificationsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin,project-manager")]
    public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationRequest request)
    {
        var command = new CreateNotificationCommand(request.UserId, request.Title, request.Message, request.Type);
        var notification = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetNotifications), new { id = notification.Id }, notification);
    }

    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkAsRead(MarkNotificationsRequest request)
    {
        var command = new MarkNotificationsAsReadCommand(request.NotificationIds);
        await _mediator.Send(command);
        return NoContent();
    }
}
