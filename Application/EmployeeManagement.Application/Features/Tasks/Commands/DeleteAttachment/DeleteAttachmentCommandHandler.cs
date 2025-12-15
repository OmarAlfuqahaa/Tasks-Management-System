using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Tasks.Commands.DeleteAttachment
{
  public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand>
  {
    private readonly IAppDbContext _context;

    public DeleteAttachmentCommandHandler(IAppDbContext context)
    {
      _context = context;
    }

    public async Task<Unit> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
      var task = await _context.Tasks
          .Include(t => t.Attachments)
          .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

      if (task == null)
        throw new NotFoundException(nameof(TaskItem), request.TaskId);

      var attachment = task.Attachments.FirstOrDefault(a => a.Id == request.AttachmentId);
      if (attachment == null)
        throw new NotFoundException(nameof(Attachment), request.AttachmentId);

      task.Attachments.Remove(attachment);
      task.UpdatedAt = DateTime.Now; 

      await _context.SaveChangesAsync(cancellationToken);

      var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attachment.FileUrl.TrimStart('/'));
      if (File.Exists(filePath))
        File.Delete(filePath);

      return Unit.Value;
    }
  }
}
