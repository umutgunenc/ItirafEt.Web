using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class CommentHub : Hub
    {
        public Task JoinPostCommentGroup(int postId)
            => Groups.AddToGroupAsync(Context.ConnectionId, $"post-{postId}");

    }
}
