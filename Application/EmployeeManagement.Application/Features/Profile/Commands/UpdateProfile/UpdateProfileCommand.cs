using EmployeeManagement.Application.Features.Profile.DTOs;
using MediatR;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Features.Profile.Commands.UpdateProfile;

public record UpdateProfileCommand(string Name, string Email) : IRequest<ProfileResponse>;

