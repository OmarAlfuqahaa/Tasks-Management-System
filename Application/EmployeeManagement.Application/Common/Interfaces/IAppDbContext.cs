using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Attachment> Attachments { get; } 


  Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

