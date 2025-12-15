using EmployeeManagement.Application.Common.Models;
using EmployeeManagement.Application.Features.Notifications.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQuery : IRequest<PagedResult<NotificationDto>>
{
    private const int MaxPageSize = 50;
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

    public bool? IsRead { get; set; }
}

