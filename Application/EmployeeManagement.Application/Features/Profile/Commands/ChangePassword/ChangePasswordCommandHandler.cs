using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Application.Features.Profile.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ChangePasswordCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (verification == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Current password is incorrect.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("User context is missing.");
        }

        var user = await _context.Users.FindAsync(new object[] { userId.Value }, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", userId.Value);
        }

        return user;
    }
}

