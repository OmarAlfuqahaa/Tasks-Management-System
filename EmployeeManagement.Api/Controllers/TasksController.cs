using EmployeeManagement.Application.Common.Models;
using EmployeeManagement.Application.Features.Tasks.Commands.CreateTask;
using EmployeeManagement.Application.Features.Tasks.Commands.DeleteTask;
using EmployeeManagement.Application.Features.Tasks.Commands.UpdateTask;
using EmployeeManagement.Application.Features.Tasks.DTOs;
using EmployeeManagement.Application.Features.Tasks.Queries.GetTaskById;
using EmployeeManagement.Application.Features.Tasks.Queries.GetTasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Application.Features.Tasks.Commands.AddComment;
using EmployeeManagement.Application.Features.Tasks.Commands.AddAttachment;
using EmployeeManagement.Application.Features.Tasks.Queries.GetCommentsByTaskId;
using EmployeeManagement.Application.Features.Tasks.Commands.DeleteAttachment;
using EmployeeManagement.Api.Hubs;
using Microsoft.AspNetCore.SignalR;


using EmployeeManagement.Domain.Entities; 





namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
  private readonly IHubContext<DashboardHub> _hubContext;




  public TasksController(IMediator mediator,ICurrentUserService currentUserService,
    IHubContext<DashboardHub> hubContext)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _hubContext = hubContext;


  }

[HttpGet]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks([FromQuery] GetTasksQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetTask(int id)
    {
        var task = await _mediator.Send(new GetTaskByIdQuery(id));
        return Ok(task);
    }

    [HttpPost]
    [Authorize(Roles = "admin,project-manager")]
    public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskRequest request)
    {
        var command = new CreateTaskCommand(request.Title, request.Description, request.Status, request.AssignedUserId);
        var task = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

  [HttpPost("{taskId}/comments")]
  public async Task<IActionResult> AddComment(int taskId, [FromBody] AddCommentRequest request)
  {
    var task = await _mediator.Send(new GetTaskByIdQuery(taskId));
    if (task == null) return NotFound();

    if (!_currentUserService.UserId.HasValue)
      return BadRequest("User not authenticated.");

    var userId = _currentUserService.UserId.Value; 

    var command = new AddCommentCommand(taskId, userId, request.Text);
    var comment = await _mediator.Send(command);

    return Ok(comment);
  }

  [HttpGet("{taskId}/comments")]
  public async Task<ActionResult<List<CommentDto>>> GetComments(int taskId)
  {
    var comments = await _mediator.Send(new GetCommentsByTaskIdQuery(taskId));
    return Ok(comments);
  }





  [HttpPost("{taskId}/attachments")]
  public async Task<IActionResult> UploadAttachment(int taskId, [FromForm] IFormFile file)
  {
    if (file == null || file.Length == 0)
      return BadRequest("No file uploaded.");

    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
    if (!Directory.Exists(uploadsFolder))
      Directory.CreateDirectory(uploadsFolder);

    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
    var filePath = Path.Combine(uploadsFolder, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
      await file.CopyToAsync(stream);
    }

    var command = new AddAttachmentCommand(
        taskId,
        file.FileName,
        $"/uploads/{fileName}",
        _currentUserService.UserId!.Value
    );

    var attachmentDto = await _mediator.Send(command);

    return Ok(attachmentDto);
  }



  [HttpPut("{id:int}")]
  public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
  {
    var command = new UpdateTaskCommand(id, request.Title, request.Description, request.Status, request.AssignedUserId);
    await _hubContext.Clients.All.SendAsync("DashboardUpdated");

    var updatedTask = await _mediator.Send(command);

    var taskDto = new TaskDto
    {
      Id = updatedTask.Id,
      Title = updatedTask.Title,
      Description = updatedTask.Description,
      Status = updatedTask.Status,
      AssignedUserId = updatedTask.AssignedUserId,
      AssignedTo = updatedTask.AssignedUser?.Name,
      CreatedAt = updatedTask.CreatedAt,
      UpdatedAt = updatedTask.UpdatedAt,
      Attachments = updatedTask.Attachments?.Select(a => new AttachmentDto
      {
        Id = a.Id,
        TaskId = a.TaskId,
        Name = a.FileName,
        Url = a.FileUrl,
        UploadedById = a.UploadedById
      }).ToList()
    };

    return Ok(taskDto);
  }



  [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,project-manager")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _mediator.Send(new DeleteTaskCommand(id));
        return NoContent();
    }

  [HttpDelete("{taskId}/attachments/{attachmentId}")]
  public async Task<IActionResult> DeleteAttachment(int taskId, int attachmentId)
  {
    await _mediator.Send(new DeleteAttachmentCommand(taskId, attachmentId));
    return NoContent();
  }


}
