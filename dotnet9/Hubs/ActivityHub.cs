using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace MusicHubs.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    public async Task BroadcastActivity(string username, string action, string musicName)
    {
        await Clients.All.SendAsync("ReceiveActivity", username, action, musicName);
    }
}
