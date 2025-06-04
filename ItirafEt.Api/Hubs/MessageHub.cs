using System.Text.RegularExpressions;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.HubServices;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class MessageHub :Hub
    {

        public Task JoinMessageGroup(Guid conversationId) => Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");       
        
        public Task JoinMessageReadGroup(Guid currentUserId) => Groups.AddToGroupAsync(Context.ConnectionId, $"user-{currentUserId}");

        public Task JoinInboxGroup(Guid currentUserId) => Groups.AddToGroupAsync(Context.ConnectionId, $"userInbox-{currentUserId}");


        //public Task JoinMessageGroup(Guid conversationId) =>
        //    Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");

    }
}
