using System.Linq;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Notifications.Commands.MarkNotificationsAsRead;

public class MarkNotificationsAsReadCommandHandler : IRequestHandler<MarkNotificationsAsReadCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationsAsReadCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(MarkNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        if (request.NotificationIds.Count == 0)
        {
            throw new BadRequestException("No notification ids provided.");
        }

        var userId = _currentUser.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("User context is missing.");
        }

        var notifications = await _context.Notifications
            .Where(n => request.NotificationIds.Contains(n.Id) && n.UserId == userId.Value)
            .ToListAsync(cancellationToken);

        if (notifications.Count == 0)
        {
            throw new NotFoundException("Notification", string.Join(",", request.NotificationIds));
        }

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

