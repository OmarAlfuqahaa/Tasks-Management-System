using EmployeeManagement.Application.Features.Auth.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;

