using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.Features.Tasks.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Status { get; set; }
    public int AssignedUserId { get; set; }
    public required string AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }  


  public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();


}

public class CreateTaskRequest
{
    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [RegularExpression("todo|in-progress|done")]
    public string Status { get; set; } = "todo";

    [Required]
    public int AssignedUserId { get; set; }
}

public class UpdateTaskRequest
{
    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [RegularExpression("todo|in-progress|done")]
    public string Status { get; set; } = "todo";

    [Required]
    public int AssignedUserId { get; set; }
}

public class CommentDto
{
  public int Id { get; set; }
  public int TaskId { get; set; }
  public int AuthorId { get; set; }
  public required string AuthorName { get; set; }
  public required string Text { get; set; }
  public DateTime CreatedAt { get; set; }
}


public class AttachmentDto
{
  public int Id { get; set; }
  public int TaskId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Url { get; set; } = string.Empty;
  public int UploadedById { get; set; }
}




