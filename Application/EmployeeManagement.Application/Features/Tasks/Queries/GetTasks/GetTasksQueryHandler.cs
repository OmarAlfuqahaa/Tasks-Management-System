using AutoMapper;
using AutoMapper.QueryableExtensions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Common.Models;
using EmployeeManagement.Application.Features.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PagedResult<TaskDto>>
{
  private readonly IAppDbContext _context;
  private readonly IMapper _mapper;
  private readonly ICurrentUserService _currentUser;

  public GetTasksQueryHandler(
      IAppDbContext context,
      IMapper mapper,
      ICurrentUserService currentUser)
  {
    _context = context;
    _mapper = mapper;
    _currentUser = currentUser;
  }

  public async Task<PagedResult<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
  {
    var query = _context.Tasks
        .AsNoTracking()
        .Include(t => t.AssignedUser)
        .Where(t => !t.IsDeleted)
        .AsQueryable();

    var role = _currentUser.Role;
    var userId = _currentUser.UserId;
    var isManager = role is "admin" or "project-manager";

    if (!isManager && userId.HasValue)
    {
      if (!request.ShowAllTasks)
      {
        query = query.Where(t => t.AssignedUserId == userId.Value);
      }
    }


    if (request.AssignedUserIds != null && request.AssignedUserIds.Any())
    {
      query = query.Where(t => request.AssignedUserIds.Contains(t.AssignedUserId));
    }

    if (request.OtherAssignees != null && request.OtherAssignees.Any())
    {
      query = query.Where(t => request.OtherAssignees.Contains(t.AssignedUserId));
    }

    if (!string.IsNullOrWhiteSpace(request.Status))
    {
      query = query.Where(t => t.Status == request.Status);
    }

    if (!string.IsNullOrWhiteSpace(request.Search))
    {
      var pattern = $"%{request.Search.Trim()}%";
      query = query.Where(t =>
          EF.Functions.Like(t.Title, pattern) ||
          (t.Description != null && EF.Functions.Like(t.Description, pattern))
      );
    }

    var totalItems = await query.CountAsync(cancellationToken);

    var items = await query
    .OrderByDescending(t => t.CreatedAt)
    .Skip((request.Page - 1) * request.PageSize)
    .Take(request.PageSize)
    .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
    .ToListAsync(cancellationToken);

    return new PagedResult<TaskDto>
    {
      Items = items,
      TotalItems = totalItems,
      Page = request.Page,
      PageSize = request.PageSize
    };

  }
}
