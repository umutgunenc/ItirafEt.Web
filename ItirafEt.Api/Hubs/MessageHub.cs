using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class MessageHub :Hub
    {
        public Task JoinMessageGroup(Guid senderUserId, Guid receiverUserId) => Groups.AddToGroupAsync(Context.ConnectionId, $"message-sender-{senderUserId}-reciver-{receiverUserId}");

    }
}
