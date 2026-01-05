using Microsoft.AspNetCore.SignalR;

namespace Youtube_Entertainment_Project.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var userId = Context.UserIdentifier;
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                }
            }
            await base.OnConnectedAsync();
        }
    }
}