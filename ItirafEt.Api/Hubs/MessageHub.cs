using System.Text.RegularExpressions;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.HubServices;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class MessageHub :Hub
    {

        public Task JoinMessageGroup(Guid conversationId) => Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");


        //public Task JoinMessageGroup(Guid conversationId) =>
        //    Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");

    }
}
