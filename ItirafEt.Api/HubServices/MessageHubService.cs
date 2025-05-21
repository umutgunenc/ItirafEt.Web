using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
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
        public async Task SendMessageAsync(Guid conversationId, MessageDto messageDto)
        {
            await _hubContext.Clients
                .Group($"conversation-{conversationId}")
                .SendAsync("SendMessageAsync", conversationId, messageDto);
        }

        public async Task SendMessageNotificationAsync(Guid conversationId, MessageDto messageDto)
        {
            await _hubContext.Clients
                .Group($"conversation-{conversationId}")
                .SendAsync("SendMessageNotificationAsync", conversationId, messageDto);
        }

        //public async Task SendMessageNotificationAsync(Guid conversationId,MessageDto messageDto,string connectionId) 
        //{
        //    await _hubContext.Clients
        //        .GroupExcept($"conversation-{conversationId}", connectionId)
        //        .SendAsync("SendMessageNotificationAsync", conversationId, messageDto);
        //}
    }
}

