using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class MemberHub : Hub
    {
        private readonly ILogger<MemberHub> _logger;
        public MemberHub(ILogger<MemberHub> logger)
        {
            _logger = logger;
        }
        public async Task SendMember(string providerId)
        {
            // Log an informational message indicating that a member is being sent for a specific providerId
            _logger.LogInformation("Sending Member for providerId" + providerId);
            // Send a signal to all clients in the specified group (providerId) to receive the member information
            await Clients.Groups(providerId).SendAsync("ReceiveMember", providerId);
        }
        public async Task JoinChannel(string providerId)
        {
            // Log an informational message indicating that the connection is joining a channel for a specific providerId
            _logger.LogInformation("Joining Channel for providerId in memberHub " + providerId);
            // Add the connection (identified by its ConnectionId) to the specified group (providerId)
            await Groups.AddToGroupAsync(Context.ConnectionId, providerId);
        }
    }
}