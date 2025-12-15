using AutoMapper;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Profile.DTOs;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UpdateProfileCommandHandler(IAppDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ProfileResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != user.Id, cancellationToken);

            if (exists)
            {
                throw new ConflictException("Email already in use.");
            }
        }

        user.Name = request.Name;
        user.Email = request.Email;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProfileResponse>(user);
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

