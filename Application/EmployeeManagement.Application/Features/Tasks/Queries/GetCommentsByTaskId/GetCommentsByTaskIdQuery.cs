using EmployeeManagement.Application.Features.Tasks.DTOs;
using MediatR;
using System.Collections.Generic;

namespace EmployeeManagement.Application.Features.Tasks.Queries.GetCommentsByTaskId
{
  public record GetCommentsByTaskIdQuery(int TaskId) : IRequest<List<CommentDto>>;
}
