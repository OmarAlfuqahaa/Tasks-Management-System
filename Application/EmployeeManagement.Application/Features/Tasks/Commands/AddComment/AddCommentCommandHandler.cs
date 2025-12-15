using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Tasks.DTOs;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace EmployeeManagement.Application.Features.Tasks.Commands.AddComment
{
  public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDto>
  {
    private readonly IAppDbContext _context;

    public AddCommentCommandHandler(IAppDbContext context)
    {
      _context = context;
    }

    public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
      var task = await _context.Tasks
          .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

      if (task == null)
        throw new NotFoundException(nameof(TaskItem), request.TaskId);

      var comment = new Comment
      {
        TaskId = request.TaskId,
        AuthorId = request.AuthorId,
        Text = request.Text,
        CreatedAt = DateTime.Now
      };

      _context.Comments.Add(comment);

      task.UpdatedAt = DateTime.Now;

      await _context.SaveChangesAsync(cancellationToken);

      var author = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == request.AuthorId, cancellationToken);

      return new CommentDto
      {
        Id = comment.Id,
        TaskId = comment.TaskId,
        AuthorId = comment.AuthorId,
        AuthorName = author?.Name ?? "Unknown User",
        Text = comment.Text,
        CreatedAt = comment.CreatedAt
      };
    }

  }

}
