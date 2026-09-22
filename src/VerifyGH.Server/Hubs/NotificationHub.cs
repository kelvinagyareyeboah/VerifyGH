using Microsoft.AspNetCore.SignalR;

namespace VerifyGH.Server.Hubs;

public interface INotificationClient
{
    Task ReceiveNotification(string title, string message, string type);
    Task ProjectStatusUpdated(int projectId, string status, string? feedback);
}

public class NotificationHub : Hub<INotificationClient>
{
    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
    }

    public async Task LeaveUserGroup(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
    }

    public async Task SendStatusUpdate(string userId, int projectId, string status, string? feedback)
    {
        await Clients.Group($"User_{userId}").ProjectStatusUpdated(projectId, status, feedback);
    }
}
