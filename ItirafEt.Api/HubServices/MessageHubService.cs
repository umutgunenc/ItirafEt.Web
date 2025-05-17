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
        //public async Task SendMessageAsync(Guid senderUserId, Guid receiverUserId, MessageDto)
        //{
        //    await _hubContext.Clients
        //        .Group($"message-sender-{senderUserId}-reciver-{receiverUserId}")
        //        .SendAsync("SendMessageAsync", senderUserId, receiverUserId, MessageDto);
        //}
    }
}
