using AutoMapper;
using EmployeeManagement.Application.Features.Profile.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Features.Profile;

public class ProfileMappingProfile : AutoMapper.Profile
{
    public ProfileMappingProfile()
    {
        CreateMap<User, ProfileResponse>();
    }
}
