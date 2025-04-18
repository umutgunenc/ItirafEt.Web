using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class CommentHub : Hub
    {
        public Task JoinPostGroup(int postId)
            => Groups.AddToGroupAsync(Context.ConnectionId, $"post-{postId}");

        public async Task NotifyCommentAdded(int postId, int commentId)
        {
            await Clients.Group($"post-{postId}")
                         .SendAsync("CommentAdded", commentId);
        }

        
        //public async Task NotifyCommentUpdated(int postId, int commentId)
        //{
        //    await Clients.All.SendAsync("CommentUpdated", postId, commentId);
        //}
        //public async Task NotifyCommentDeleted(int postId, int commentId)
        //{
        //    await Clients.All.SendAsync("CommentDeleted", postId, commentId);
        //}

    }
}
