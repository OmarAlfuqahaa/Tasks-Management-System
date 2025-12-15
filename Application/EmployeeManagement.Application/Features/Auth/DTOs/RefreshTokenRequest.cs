using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.Features.Auth.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

