using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.Features.Users.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateUserRequest
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("admin|project-manager|employee", ErrorMessage = "Role must be admin, project-manager or employee.")]
    public string Role { get; set; } = "employee";
}

