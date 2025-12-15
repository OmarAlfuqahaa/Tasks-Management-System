using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EmployeeManagement.Infrastructure
{
  public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
  {
    public AppDbContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

      var connectionString = "Server=localhost;Port=3306;Database=EmployeeManagement;User=root;Password=root;TreatTinyAsBoolean=false";
      var serverVersion = ServerVersion.AutoDetect(connectionString);

      optionsBuilder.UseMySql(connectionString, serverVersion);

      return new AppDbContext(optionsBuilder.Options);
    }
  }
}
