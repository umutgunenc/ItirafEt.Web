using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;

namespace ItirafEt.Api.Hubs
{
    public class ReactionHub : Hub
    {
        public Task JoinPostReactionGroup(int postId) => Groups.AddToGroupAsync(Context.ConnectionId, $"post-{postId}");
        
        public Task JoinCategoryPostReactionGroup(int categoryId) => Groups.AddToGroupAsync(Context.ConnectionId, $"category-{categoryId}");

        public Task JoinPostCommentReactionGroup(int postId) => Groups.AddToGroupAsync(Context.ConnectionId, $"postCommentReactionGroup-{postId}");


    }
}