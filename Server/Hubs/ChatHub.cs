using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }
        public async Task SendMessage(string message , string providerId , string channelId , string memberId)
        {
            _logger.LogInformation  ("now sending message");
            _logger.LogInformation  ($"providerId {providerId} , channelId {channelId} , memberId {memberId}");
            await Clients.All.SendAsync("ReceiveMessage", message , providerId , channelId , memberId);
        }
        public async Task JoinChannel(string groupName)
        {
            _logger.LogInformation("join channel for sending message in group " + groupName);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendTyping (string providerId , string channelId , string memberId)
        {
            _logger.LogInformation($"Sending typing message to member {memberId}");
            await Clients.Groups(providerId).SendAsync( "ReceiveTyping" , providerId , channelId , memberId);
        }
    }
}
