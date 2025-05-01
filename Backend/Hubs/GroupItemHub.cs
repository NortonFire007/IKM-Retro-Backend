using Microsoft.AspNetCore.SignalR;

namespace IKM_Retro.Hubs;

public class GroupItemHub : Hub
{
    public async Task SendGroupItemUpdate(string retrospectiveId, object payload)
    {
        await Clients.Group(retrospectiveId).SendAsync("ReceiveGroupItemUpdate", payload);
    }

    public override async Task OnConnectedAsync()
    {
        var retrospectiveId = Context.GetHttpContext()?.Request.Query["retrospectiveId"];
        if (!string.IsNullOrEmpty(retrospectiveId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, retrospectiveId!);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var retrospectiveId = Context.GetHttpContext()?.Request.Query["retrospectiveId"];
        if (!string.IsNullOrEmpty(retrospectiveId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, retrospectiveId!);
        }

        await base.OnDisconnectedAsync(exception);
    }
}