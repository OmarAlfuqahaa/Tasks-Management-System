using AutoMapper;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Tasks.DTOs;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
  private readonly IAppDbContext _context;
  private readonly IMapper _mapper;
  private readonly ICurrentUserService _currentUser;

  public GetTaskByIdQueryHandler(
      IAppDbContext context,
      IMapper mapper,
      ICurrentUserService currentUser)
  {
    _context = context;
    _mapper = mapper;
    _currentUser = currentUser;
  }

  public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
  {
    // جلب التاسك مع المستخدم والمرفقات
    var task = await _context.Tasks
        .Include(t => t.AssignedUser)
        .Include(t => t.Attachments)
        .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

    if (task is null)
      throw new NotFoundException(nameof(TaskItem), request.Id);

    if (!CanAccess(task))
      throw new ForbiddenAccessException();

    // تحويل التاسك إلى DTO
    var taskDto = _mapper.Map<TaskDto>(task);

    // إضافة المرفقات
    taskDto.Attachments = task.Attachments.Select(a => new AttachmentDto
    {
      Id = a.Id,
      TaskId = a.TaskId,
      Name = a.FileName,
      Url = a.FileUrl,
      UploadedById = a.UploadedById
    }).ToList();

    return taskDto;
  }

  private bool CanAccess(TaskItem task)
  {
    var role = _currentUser.Role;
    if (role is "admin" or "project-manager")
    {
      return true;
    }

    return _currentUser.UserId.HasValue && task.AssignedUserId == _currentUser.UserId.Value;
  }
}
