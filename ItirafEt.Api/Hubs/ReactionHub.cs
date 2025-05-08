using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class ReactionHub : Hub
    {
        public Task JoinPostReactionGroup(int postId) => Groups.AddToGroupAsync(Context.ConnectionId, $"post-{postId}");

    }

}
