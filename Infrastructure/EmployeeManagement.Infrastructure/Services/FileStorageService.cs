using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Services
{
  public interface IFileStorageService
  {
    Task<string> SaveFileAsync(IFormFile file);
  }

  public class FileStorageService : IFileStorageService
  {
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env)
    {
      _env = env;
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
      if (file == null || file.Length == 0)
        throw new ArgumentException("No file provided");

      var uploadsFolder = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
      if (!Directory.Exists(uploadsFolder))
        Directory.CreateDirectory(uploadsFolder);

      var fileName = $"{Guid.NewGuid()}_{file.FileName}";
      var filePath = Path.Combine(uploadsFolder, fileName);

      using (var stream = new FileStream(filePath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      return $"/uploads/{fileName}";
    }
  }
}
