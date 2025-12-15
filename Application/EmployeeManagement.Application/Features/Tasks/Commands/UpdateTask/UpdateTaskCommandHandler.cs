using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace EmployeeManagement.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskItem>
{
  private readonly IAppDbContext _context;
  private readonly ICurrentUserService _currentUser;

  public UpdateTaskCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
  {
    _context = context;
    _currentUser = currentUser;
  }

  public async Task<TaskItem> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
  {
    var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
    if (task is null)
      throw new NotFoundException(nameof(TaskItem), request.Id);

    if (!CanModify(task))
      throw new ForbiddenAccessException();

    var assignee = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.AssignedUserId, cancellationToken);
    if (assignee is null)
      throw new BadRequestException("Assigned user not found.");

    if (!string.Equals(assignee.Role, "employee", StringComparison.OrdinalIgnoreCase))
      throw new BadRequestException("Cannot assign tasks to Admin or Project Manager users.");

    task.Title = request.Title;
    task.Description = request.Description;
    task.Status = request.Status;
    task.AssignedUserId = request.AssignedUserId;
    task.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync(cancellationToken);

    return task; // ترجع المهمة بعد التحديث
  }

  private bool CanModify(TaskItem task)
  {
    var role = _currentUser.Role;
    if (role is "admin" or "project-manager")
      return true;

    return role == "employee" &&
           _currentUser.UserId.HasValue &&
           task.AssignedUserId == _currentUser.UserId.Value;
  }
}

