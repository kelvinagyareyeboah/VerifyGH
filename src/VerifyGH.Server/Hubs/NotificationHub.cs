using Microsoft.AspNetCore.SignalR;
using VerifyGH.Shared.DTOs;

namespace VerifyGH.Server.Hubs;

public interface INotificationClient
{
    Task ReceiveNotification(string title, string message, string type);
    Task ProjectStatusUpdated(int projectId, string status, string? feedback);
    Task NewProjectSubmitted(ProjectDto project);
    Task ProjectDeleted(int projectId);
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
        await Clients.All.ProjectStatusUpdated(projectId, status, feedback);
    }

    public async Task BroadcastNewProject(ProjectDto project)
    {
        await Clients.All.NewProjectSubmitted(project);
    }

    public async Task BroadcastProjectDeleted(int projectId)
    {
        await Clients.All.ProjectDeleted(projectId);
    }
}
