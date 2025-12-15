using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Auth.DTOs;
using EmployeeManagement.Application.Features.Auth.Services;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IAuthTokenFactory _tokenFactory;

    public LoginCommandHandler(
        IAppDbContext context,
        IPasswordHasher<User> passwordHasher,
        IAuthTokenFactory tokenFactory)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenFactory = tokenFactory;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            await Task.Delay(Random.Shared.Next(100, 300), cancellationToken);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return await _tokenFactory.CreateAsync(user, cancellationToken);
    }
}
