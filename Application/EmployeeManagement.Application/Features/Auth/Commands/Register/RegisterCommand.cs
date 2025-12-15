using EmployeeManagement.Application.Features.Auth.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Name, string Email, string Password, string Role) : IRequest<AuthResponse>;

