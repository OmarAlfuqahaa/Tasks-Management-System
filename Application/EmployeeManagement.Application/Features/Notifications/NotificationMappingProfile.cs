using AutoMapper;
using EmployeeManagement.Application.Features.Notifications.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Features.Notifications;

public class NotificationMappingProfile : AutoMapper.Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}

