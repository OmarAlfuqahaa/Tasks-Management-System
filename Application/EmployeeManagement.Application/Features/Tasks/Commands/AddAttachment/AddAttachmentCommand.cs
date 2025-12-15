using EmployeeManagement.Application.Features.Tasks.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagement.Application.Features.Tasks.Commands.AddAttachment
{
  public record AddAttachmentCommand(
          int TaskId,
          string FileName,
          string FileUrl,
          int UploadedById
      ) : IRequest<AttachmentDto>;
}
