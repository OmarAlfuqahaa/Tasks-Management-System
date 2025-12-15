using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.Features.Auth.DTOs;

public class RegisterRequest
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("admin|project-manager|employee", ErrorMessage = "Role must be admin, project-manager or employee.")]
    public string Role { get; set; } = "employee";
}

