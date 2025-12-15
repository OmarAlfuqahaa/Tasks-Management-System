using System.Collections.Generic;
using MediatR;

namespace EmployeeManagement.Application.Features.Notifications.Commands.MarkNotificationsAsRead;

public record MarkNotificationsAsReadCommand(IReadOnlyCollection<int> NotificationIds) : IRequest<Unit>;

