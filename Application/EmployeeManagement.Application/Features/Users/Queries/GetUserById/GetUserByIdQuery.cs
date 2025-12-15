using EmployeeManagement.Application.Features.Users.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;

