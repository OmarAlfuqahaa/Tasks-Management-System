using EmployeeManagement.Application.Features.Tasks.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(string Title, string? Description, string Status, int AssignedUserId) : IRequest<TaskDto>;

