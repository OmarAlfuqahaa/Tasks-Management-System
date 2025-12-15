using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        if (!context.Users.Any())
        {
            var admin = new User
            {
                Name = "Admin",
                Email = "admin@example.com",
                Role = "admin",
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");

            context.Users.Add(admin);

            await context.SaveChangesAsync();
        }
    }
}
