using MediatR;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Features.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand(int Id, string Title, string? Description, string Status, int AssignedUserId)
    : IRequest<TaskItem>;
