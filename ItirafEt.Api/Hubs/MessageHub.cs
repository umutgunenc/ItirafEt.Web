using System.Text.RegularExpressions;
using ItirafEt.Api.Data.Entities;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class MessageHub :Hub
    {
        public Task JoinMessageGroup(Guid conversationId) => Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");

    }
}
