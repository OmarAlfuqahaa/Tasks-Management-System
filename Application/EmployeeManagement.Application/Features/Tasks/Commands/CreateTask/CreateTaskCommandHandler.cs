using AutoMapper;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Features.Tasks.DTOs;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public CreateTaskCommandHandler(IAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var assignee = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.AssignedUserId, cancellationToken);

        if (assignee is null || !string.Equals(assignee.Role, "employee", StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException("Assigned user must be a valid employee.");
        }

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            AssignedUserId = request.AssignedUserId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        task.AssignedUser = assignee;
        return _mapper.Map<TaskDto>(task);
    }
}

