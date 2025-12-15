using MediatR;

namespace EmployeeManagement.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Unit>;

