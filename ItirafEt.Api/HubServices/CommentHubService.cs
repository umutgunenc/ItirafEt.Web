using ItirafEt.Api.Hubs;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.HubServices
{
    public class CommentHubService
    {

        private readonly IHubContext<CommentHub> _hubContext;
        public CommentHubService(IHubContext<CommentHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task CommentAddedOrDeletedAsync(int postId, CommentsViewModel model, bool isAdded)
        {
            await _hubContext.Clients
                .Group($"post-{postId}")
                .SendAsync("CommentAddedOrDeletedAsync", model, isAdded);
        }

        public async Task ReplyAddedOrDeletedAsync(int postId, CommentsViewModel replyDto, bool isAdded)
        {
            await _hubContext.Clients
                .Group($"post-{postId}")
                .SendAsync("ReplyAddedOrDeletedAsync", replyDto, isAdded);
        }

    }
}
