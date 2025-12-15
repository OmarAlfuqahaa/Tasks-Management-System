using AutoMapper;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Profile.DTOs;
using EmployeeManagement.Domain.Entities;
using MediatR;

namespace EmployeeManagement.Application.Features.Profile.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetProfileQueryHandler(IAppDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
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

