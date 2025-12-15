using EmployeeManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Domain.Entities;
using System.Threading;
using EmployeeManagement.Api.Hubs;
using Microsoft.AspNetCore.SignalR;


namespace EmployeeManagement.Api.Controllers
{
  [ApiController]
  [Route("api/dashboard")]
  public class DashboardController : ControllerBase
  {
    private readonly IAppDbContext _context;
    private readonly IHubContext<DashboardHub> _hubContext;


    public DashboardController(IAppDbContext context, IHubContext<DashboardHub> hubContext)
    {
      _context = context;
      _hubContext = hubContext;
    }

    [HttpGet("welcome")]
    public async Task<IActionResult> GetWelcomeMessage()
    {
      // ------- USER COUNTS ---------
      var employees = await _context.Users
          .CountAsync(u => u.Role == "employee" && !u.IsDeleted);

      var projectManagers = await _context.Users
          .CountAsync(u => u.Role == "project-manager" && !u.IsDeleted);

      // ------- TASK COUNTS ---------
      var todo = await _context.Tasks
          .CountAsync(t => t.Status.ToLower() == "todo" && !t.IsDeleted);

      var inProgress = await _context.Tasks
          .CountAsync(t => t.Status.ToLower() == "in-progress" && !t.IsDeleted);

      var done = await _context.Tasks
          .CountAsync(t => t.Status.ToLower() == "done" && !t.IsDeleted);

      // ------- TASK COUNTS PER EMPLOYEE  ---------
      var taskCounts = await _context.Tasks
          .Where(t => !t.IsDeleted)
          .GroupBy(t => t.AssignedUserId)
          .Select(g => new
          {
            UserId = g.Key,
            TodoCount = g.Count(t => t.Status.ToLower() == "todo"),
            InProgressCount = g.Count(t => t.Status.ToLower() == "in-progress"),
            DoneCount = g.Count(t => t.Status.ToLower() == "done"),
            TotalTasks = g.Count()
          })
          .ToListAsync();

      // ------- EMPLOYEES WITH / WITHOUT ACTIVE TASKS (Todo / In Progress) ---------
      var employeesWithActiveTasks = await _context.Users
          .Where(u => u.Role == "employee" && !u.IsDeleted)
          .Include(u => u.Tasks)
          .CountAsync(u => u.Tasks.Any(t => !t.IsDeleted && (t.Status.ToLower() == "todo" || t.Status.ToLower() == "in-progress")));

      var employeesWithoutActiveTasks = await _context.Users
          .Where(u => u.Role == "employee" && !u.IsDeleted)
          .Include(u => u.Tasks)
          .CountAsync(u => !u.Tasks.Any(t => !t.IsDeleted) || u.Tasks.All(t => t.Status.ToLower() == "done"));

      var hubMessage = $"You have: 📝 {todo} Todo • ⚙️ {inProgress} In Progress • ✅ {done} Done";

      

      return Ok(new
      {
        employees,
        projectManagers,
        employeesWithActiveTasks,
        employeesWithoutActiveTasks,
        todo,
        inProgress,
        done,
        taskCounts
      });
    }

    [HttpGet("idle-employees")]
    public async Task<IActionResult> GetIdleEmployees()
    {
      var employees = await _context.Users
          .Where(u => u.Role == "employee" && !u.IsDeleted)
          .Include(u => u.Tasks)
          .ToListAsync();

          var idleEmployees = employees
             .Select(u => {
               var activeTasks = u.Tasks.Where(t => !t.IsDeleted).ToList(); 
               return new
               {
                 Id = u.Id,
                 Name = u.Name,
                 Email = u.Email,
                 TotalTasks = activeTasks.Count,
                 Status = !activeTasks.Any() ? "IDLE"
                            : activeTasks.All(t => t.Status.ToLower() == "done") ? "DONE"
                            : "ACTIVE"
               };
             })
        .Where(e => e.Status == "IDLE" || e.Status == "DONE") 
        .ToList();


      return Ok(idleEmployees);
    }


    [HttpPost("assign-task")]
    public async Task<IActionResult> AssignTaskToEmployee([FromBody] AssignTaskDto dto)
    {
      var employee = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == dto.EmployeeId && !u.IsDeleted);

      if (employee == null) return NotFound("Employee not found.");

      var task = new TaskItem
      {
        Title = dto.Title,
        Description = dto.Description,
        Status = dto.Status ?? "todo",
        AssignedUserId = employee.Id,
        CreatedAt = DateTime.UtcNow
      };

      _context.Tasks.Add(task);
      await _context.SaveChangesAsync(CancellationToken.None); 

      return Ok(task);
    }

    // DTO
    public class AssignTaskDto
    {
      public int EmployeeId { get; set; }
      public string Title { get; set; } = string.Empty;
      public string? Description { get; set; }
      public string? Status { get; set; }
    }
  }
}
