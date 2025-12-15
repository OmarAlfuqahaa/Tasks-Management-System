using EmployeeManagement.Application.Features.Auth.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Features.Auth.Services;

public interface IAuthTokenFactory
{
    Task<AuthResponse> CreateAsync(User user, CancellationToken cancellationToken);
}

