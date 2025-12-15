using EmployeeManagement.Application.Common.Models;
using EmployeeManagement.Application.Features.Users.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<PagedResult<UserDto>>
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

    public string? Role { get; set; }
    public string? Search { get; set; }
}

