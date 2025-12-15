using AutoMapper;
using EmployeeManagement.Application.Features.Tasks.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Features.Tasks;

public class TaskMappingProfile : AutoMapper.Profile
{
  public TaskMappingProfile()
  {
    CreateMap<TaskItem, TaskDto>()
        .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedUser != null ? src.AssignedUser.Name : string.Empty));

    CreateMap<Attachment, AttachmentDto>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FileName))
        .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.FileUrl));

    CreateMap<Comment, CommentDto>()
        .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.Name : string.Empty));
  }
}
