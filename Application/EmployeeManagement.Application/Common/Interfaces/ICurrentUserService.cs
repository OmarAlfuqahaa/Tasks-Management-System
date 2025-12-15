namespace EmployeeManagement.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Role { get; }
    string? Email { get; }
    bool IsInRole(string role);
}

