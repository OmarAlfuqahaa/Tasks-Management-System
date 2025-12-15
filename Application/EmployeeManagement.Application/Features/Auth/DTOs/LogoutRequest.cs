using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.Features.Auth.DTOs;

public class LogoutRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
