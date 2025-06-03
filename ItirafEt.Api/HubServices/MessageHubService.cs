using ItirafEt.Api.Hubs;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.HubServices
{
    public class MessageHubService
    {
        private readonly IHubContext<MessageHub> _hubContext;
        public MessageHubService(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendMessageAsync(Guid conversationId, MessageViewModel model)
        {
            await _hubContext.Clients
                .Group($"conversation-{conversationId}")
                .SendAsync("SendMessageAsync", conversationId, model);
        }

        public async Task SendMessageNotificationAsync(Guid conversationId, MessageViewModel model)
        {
            await _hubContext.Clients
                .Group($"conversation-{conversationId}")
                .SendAsync("SendMessageNotificationAsync", conversationId, model);
        }
        public async Task ReadMessageAsync(Guid conversationId, MessageViewModel messageDto)
        {
            await _hubContext.Clients
                .Group($"conversation-{conversationId}")
                .SendAsync("ReadMessageAsync", conversationId, messageDto);
        }

    }
}

