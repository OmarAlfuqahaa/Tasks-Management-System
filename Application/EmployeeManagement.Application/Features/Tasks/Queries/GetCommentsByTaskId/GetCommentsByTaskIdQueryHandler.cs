using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Features.Tasks.Queries.GetCommentsByTaskId
{
  public class GetCommentsByTaskIdQueryHandler : IRequestHandler<GetCommentsByTaskIdQuery, List<CommentDto>>
  {
    private readonly IAppDbContext _context;

    public GetCommentsByTaskIdQueryHandler(IAppDbContext context)
    {
      _context = context;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsByTaskIdQuery request, CancellationToken cancellationToken)
    {
      return await _context.Comments
          .Where(c => c.TaskId == request.TaskId)
          .Include(c => c.Author) 
          .OrderBy(c => c.CreatedAt) 
          .Select(c => new CommentDto
          {
            Id = c.Id,
            TaskId = c.TaskId,
            AuthorId = c.AuthorId,
            AuthorName = c.Author!.Name,
            Text = c.Text,
            CreatedAt = c.CreatedAt
          })
          .ToListAsync(cancellationToken);
    }
  }
}
