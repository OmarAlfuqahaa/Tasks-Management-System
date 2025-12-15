using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Auth.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Features.Auth.Services;

public class AuthTokenFactory : IAuthTokenFactory
{
    private readonly ITokenService _tokenService;
    private readonly IAppDbContext _context;

    public AuthTokenFactory(ITokenService tokenService, IAppDbContext context)
    {
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<AuthResponse> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();
        refreshToken.UserId = user.Id;

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            User = new UserSummary
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            }
        };
    }
}
