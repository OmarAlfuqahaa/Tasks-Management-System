using EmployeeManagement.Application.Features.Auth.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

