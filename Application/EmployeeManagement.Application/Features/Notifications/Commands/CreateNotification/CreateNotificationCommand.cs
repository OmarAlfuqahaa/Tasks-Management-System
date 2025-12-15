using EmployeeManagement.Application.Features.Notifications.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Notifications.Commands.CreateNotification;

public record CreateNotificationCommand(int UserId, string Title, string Message, string Type) : IRequest<NotificationDto>;
