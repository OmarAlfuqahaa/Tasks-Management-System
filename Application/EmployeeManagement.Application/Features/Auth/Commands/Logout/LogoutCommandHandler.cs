using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public LogoutCommandHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, cancellationToken);

        if (storedToken is null)
        {
            throw new NotFoundException("RefreshToken", request.RefreshToken);
        }

        var userId = _currentUser.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("User context is missing.");
        }

        var isAdmin = _currentUser.IsInRole("admin");
        if (!isAdmin && storedToken.UserId != userId.Value)
        {
            throw new ForbiddenAccessException("You are not allowed to revoke this token.");
        }

        storedToken.Revoked = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
