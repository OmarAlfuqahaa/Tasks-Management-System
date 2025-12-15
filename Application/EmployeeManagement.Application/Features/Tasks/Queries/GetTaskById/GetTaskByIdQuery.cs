using EmployeeManagement.Application.Features.Tasks.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery(int Id) : IRequest<TaskDto>;

