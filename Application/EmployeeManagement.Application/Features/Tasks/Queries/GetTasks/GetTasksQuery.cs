using EmployeeManagement.Application.Common.Models;
using EmployeeManagement.Application.Features.Tasks.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Tasks.Queries.GetTasks;

public class GetTasksQuery : IRequest<PagedResult<TaskDto>>
{
  private const int MaxPageSize = 100;
  private int _pageSize = 10;
  private int _page = 1;

  public int Page
  {
    get => _page;
    set => _page = value < 1 ? 1 : value;
  }

  public int PageSize
  {
    get => _pageSize;
    set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
  }

  public string? Status { get; set; }

  public List<int>? AssignedUserIds { get; set; }

  public List<int>? OtherAssignees { get; set; }

  public string? Search { get; set; }

  public bool ShowAllTasks { get; set; } = false;

}
