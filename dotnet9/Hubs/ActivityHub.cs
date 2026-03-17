using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace MusicWebapi.Api.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    // תוספת: פונקציה שרצה אוטומטית כשמישהו מתחבר ל-Socket
    public override async Task OnConnectedAsync()
    {
        // שליפת ה-Claim שקראת לו "role" מתוך המשתמש המחובר
        var roleClaim = Context.User?.FindFirst("role")?.Value;

        // בדיקה האם הערך של ה-Claim הוא "Admin"
        if (roleClaim == "Admin")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        await base.OnConnectedAsync();
    }
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
