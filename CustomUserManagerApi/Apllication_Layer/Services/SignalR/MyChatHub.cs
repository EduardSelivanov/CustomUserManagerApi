using Microsoft.AspNetCore.SignalR;

namespace CustomUserManagerApi.Apllication_Layer.Services.SignalR
{
    public class MyChatHub : Hub
    {
        public async Task SendMessageToMyChat(string message)
        {
            await Clients.All.SendAsync("ReceivedMessageFromMyChat", message);
        }
    }
}
