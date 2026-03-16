using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace MusicWebapi.Api.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    public async Task BroadcastActivity(string username, string action, string musicName, string? userId = null)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.User(userId).SendAsync("ReceiveActivity", username, action, musicName);
        }
        else
        {
            await Clients.All.SendAsync("ReceiveActivity", username, action, musicName);
        }
    }
}
