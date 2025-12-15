using AutoMapper;
using AutoMapper.QueryableExtensions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Common.Models;
using EmployeeManagement.Application.Features.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
          var query = _context.Users
              .AsNoTracking()
              .Where(u => !u.IsDeleted) 
              .AsQueryable();

    if (!string.IsNullOrWhiteSpace(request.Role) && !string.Equals(request.Role, "All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(u => u.Role == request.Role);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var pattern = $"%{request.Search.Trim()}%";
            query = query.Where(u =>
                EF.Functions.Like(u.Name, pattern) ||
                EF.Functions.Like(u.Email, pattern));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(u => u.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserDto>
        {
            Items = items,
            TotalItems = totalItems,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}

