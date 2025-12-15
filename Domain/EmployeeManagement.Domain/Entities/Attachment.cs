namespace EmployeeManagement.Domain.Entities;

public class Attachment
{
  public int Id { get; set; }
  public int TaskId { get; set; }
  public string FileName { get; set; } = string.Empty;
  public string FileUrl { get; set; } = string.Empty;
  public int UploadedById { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public TaskItem? Task { get; set; }
}
