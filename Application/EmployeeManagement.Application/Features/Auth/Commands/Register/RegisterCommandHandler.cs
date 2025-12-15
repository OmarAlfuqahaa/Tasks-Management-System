using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Auth.DTOs;
using EmployeeManagement.Application.Features.Auth.Services;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IAuthTokenFactory _tokenFactory;

    public RegisterCommandHandler(
        IAppDbContext context,
        IPasswordHasher<User> passwordHasher,
        IAuthTokenFactory tokenFactory)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenFactory = tokenFactory;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException("Email already exists.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Role = request.Role
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return await _tokenFactory.CreateAsync(user, cancellationToken);
    }
}

