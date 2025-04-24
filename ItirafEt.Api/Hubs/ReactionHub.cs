using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class ReactionHub : Hub
    {
        public Task JoinPostReactionGroup(int postId) => Groups.AddToGroupAsync(Context.ConnectionId, $"post-{postId}");

        public async Task NotifyPostLikedOrDisliked(int postId, ReactionDto reaction, bool isReactionUpdated)
        {
            await Clients.Group($"post-{postId}")
                .SendAsync("PostLikedOrDisliked", reaction, isReactionUpdated);
        }


        //public async Task NotifyPostLikedOrDisliked()
        //{
        //    await Clients.All.SendAsync("LikedOrDisliked");
        //}
    }

}
