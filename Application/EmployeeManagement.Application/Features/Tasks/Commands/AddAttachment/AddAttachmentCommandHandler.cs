using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Tasks.DTOs;
using EmployeeManagement.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Features.Tasks.Commands.AddAttachment
{
  public class AddAttachmentCommandHandler : IRequestHandler<AddAttachmentCommand, AttachmentDto>
  {
    private readonly IAppDbContext _context;

    public AddAttachmentCommandHandler(IAppDbContext context)
    {
      _context = context;
    }

    public async Task<AttachmentDto> Handle(AddAttachmentCommand request, CancellationToken cancellationToken)
    {
      var task = await _context.Tasks
          .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

      if (task == null)
        throw new NotFoundException(nameof(TaskItem), request.TaskId);

      var attachment = new Attachment
      {
        TaskId = request.TaskId,
        FileName = request.FileName,
        FileUrl = request.FileUrl,
        UploadedById = request.UploadedById,
        CreatedAt = DateTime.Now
      };

      _context.Attachments.Add(attachment);

      task.UpdatedAt = DateTime.Now;

      await _context.SaveChangesAsync(cancellationToken);

      return new AttachmentDto
      {
        Id = attachment.Id,
        TaskId = attachment.TaskId,
        Name = attachment.FileName,
        Url = attachment.FileUrl,
        UploadedById = attachment.UploadedById
      };
    }

  }

}
