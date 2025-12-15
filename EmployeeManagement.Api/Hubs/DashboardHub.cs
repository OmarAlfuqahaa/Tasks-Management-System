using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Hubs
{
  public class DashboardHub : Hub
  {
    public async Task SendWelcomeMessage(string userId, string message)
    {
      await Clients.User(userId).SendAsync("ReceiveWelcomeMessage", message);
    }

    public async Task BroadcastWelcomeMessage(string message)
    {
      await Clients.All.SendAsync("ReceiveWelcomeMessage", message);
    }
  }
}
