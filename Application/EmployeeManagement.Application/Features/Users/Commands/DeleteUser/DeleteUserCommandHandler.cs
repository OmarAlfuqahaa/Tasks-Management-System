using EmployeeManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Features.Users.Commands.DeleteUser
{
  public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
  {
    private readonly IAppDbContext _context;

    public DeleteUserCommandHandler(IAppDbContext context)
    {
      _context = context;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
      var user = await _context.Users
          .Include(u => u.Tasks)
          .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

      if (user == null)
      {
        return false;
      }

      foreach (var task in user.Tasks)
      {
        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;
      }

      user.IsDeleted = true;
      user.DeletedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync(cancellationToken);
      return true;
    }
  }
}
