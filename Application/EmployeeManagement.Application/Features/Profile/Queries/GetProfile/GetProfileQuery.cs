using EmployeeManagement.Application.Features.Profile.DTOs;
using MediatR;

namespace EmployeeManagement.Application.Features.Profile.Queries.GetProfile;

public record GetProfileQuery : IRequest<ProfileResponse>;

