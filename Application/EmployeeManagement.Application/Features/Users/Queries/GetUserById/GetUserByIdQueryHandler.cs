using AutoMapper;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
              var user = await _context.Users
                  .AsNoTracking()
                  .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

              return user is null ? null : _mapper.Map<UserDto>(user);


    
    }
}

