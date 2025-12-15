using MediatR;

namespace EmployeeManagement.Application.Features.Tasks.Commands.DeleteAttachment
{
  public record DeleteAttachmentCommand(int TaskId, int AttachmentId) : IRequest;
}
