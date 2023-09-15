using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs;

public class GameHub : Hub
{
    public async Task NewMessage(string user, string message)
    {
        await Clients.All.SendAsync("messageReceived", user, message);
    }
}