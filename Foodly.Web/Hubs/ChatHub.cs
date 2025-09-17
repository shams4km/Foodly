using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Foodly.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            var user = Context.User?.Identity?.Name ?? "anon";
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.UtcNow.ToString("u"));
        }
    }
}
