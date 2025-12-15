using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Auth.DTOs;
using EmployeeManagement.Application.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IAppDbContext _context;
    private readonly IAuthTokenFactory _tokenFactory;

    public RefreshTokenCommandHandler(IAppDbContext context, IAuthTokenFactory tokenFactory)
    {
        _context = context;
        _tokenFactory = tokenFactory;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

        if (storedToken is null || storedToken.Revoked || storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        storedToken.Revoked = true;
        await _context.SaveChangesAsync(cancellationToken);

        if (storedToken.User is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        return await _tokenFactory.CreateAsync(storedToken.User, cancellationToken);
    }
}