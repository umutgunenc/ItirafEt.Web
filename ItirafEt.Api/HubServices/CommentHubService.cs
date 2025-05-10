using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
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
        public async Task CommentAddedOrDeletedAsync(int postId, CommentsDto dto, bool isAdded)
        {
            await _hubContext.Clients
                .Group($"post-{postId}")
                .SendAsync("CommentAddedOrDeletedAsync", dto, isAdded);
        }

        public async Task ReplyAddedOrDeletedAsync(int postId, CommentsDto replyDto, bool isAdded)
        {
            await _hubContext.Clients
                .Group($"post-{postId}")
                .SendAsync("ReplyAddedOrDeletedAsync", replyDto, isAdded);
        }

    }
}
