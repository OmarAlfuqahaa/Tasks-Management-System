using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.Features.Notifications.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public string Type { get; set; } = "info";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateNotificationRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(64)]
    public string Type { get; set; } = "info";
}

public class MarkNotificationsRequest
{
    [Required]
    [MinLength(1)]
    public int[] NotificationIds { get; set; } = Array.Empty<int>();
}
