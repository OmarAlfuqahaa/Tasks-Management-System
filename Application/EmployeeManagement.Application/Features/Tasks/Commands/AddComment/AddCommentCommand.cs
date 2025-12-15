using EmployeeManagement.Application.Features.Tasks.DTOs;
using MediatR;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Features.Tasks.Commands.AddComment
{
  public record AddCommentCommand(int TaskId, int AuthorId, string Text) : IRequest<CommentDto>;
}
