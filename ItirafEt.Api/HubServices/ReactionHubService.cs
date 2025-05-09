using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.HubServices
{
    public class ReactionHubService
    {

        private readonly IHubContext<ReactionHub> _hubContext;
        public ReactionHubService(IHubContext<ReactionHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task PostLikedOrDislikedAsync(int postId, ReactionDto reactionDto, bool isUpdated)
        {
            await _hubContext.Clients
                .Group($"post-{postId}")
                .SendAsync("PostLikedOrDislikedAsync", reactionDto, isUpdated);
        }

        public async Task UpdatePostLikeCountAsync(int categoryId, int postId, int likeCount)
        {
            await _hubContext.Clients
                .Group($"category-{categoryId}")
                .SendAsync("UpdatePostLikeCountAsync", postId, likeCount);
        }

    }
}
