using AutoMapper;
using EmployeeManagement.Application.Features.Users.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Features.Users;

public class UserMappingProfile : AutoMapper.Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}

