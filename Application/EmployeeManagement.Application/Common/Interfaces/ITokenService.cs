using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Common.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(User user);
    RefreshToken CreateRefreshToken();
}

