using MediatR;

namespace EmployeeManagement.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(int Id, string Name, string Email, string Role) : IRequest<bool>;

