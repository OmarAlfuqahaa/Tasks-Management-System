using MediatR;

namespace EmployeeManagement.Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(int Id) : IRequest;

