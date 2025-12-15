namespace EmployeeManagement.Application.Features.Auth.DTOs;

public class AuthResponse
{
    public required UserSummary User { get; init; }
    public required string Token { get; init; }
    public string? RefreshToken { get; init; }
}

public class UserSummary
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Role { get; init; }
}

