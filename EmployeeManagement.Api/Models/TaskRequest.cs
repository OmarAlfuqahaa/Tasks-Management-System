using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models
{
  public class AddAttachmentRequest
  {
    [Required]
    public IFormFile File { get; set; } = default!;
  }

  public class AddCommentRequest
  {
    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;
  }
}
