using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MusicWebapi.Api.Hubs;

[Authorize]
public class ActivityHub : Hub
{
    // רץ בכל פעם שמישהו מתחבר (מכל דף שהוא)
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier; // ה-ID של הלוגין

        if (!string.IsNullOrEmpty(userId))
        {
            // 1. נצרף אותו לקבוצה אישית (עבור דף ה-Music)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        // 2. נצרף אותו לקבוצת אדמינים (עבור דף ה-Users)
        if (Context.User.IsInRole("Admin"))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        await base.OnConnectedAsync();
    }

    // פונקציה כללית שניתן לקרוא לה מה-Service
    public async Task NotifyActivity(string targetUserId, string adminName, string action, string itemName)
    {
        // שליחה למשתמש הספציפי (לדף ה-Music שלו)
        await Clients.Group($"User_{targetUserId}").SendAsync("ReceivePersonalUpdate", action, itemName);

        // שליחה לכל המנהלים (לדף ה-Users שלהם)
        await Clients.Group("Admins").SendAsync("ReceiveAdminAlert", adminName, action, itemName, targetUserId);
    }
}
