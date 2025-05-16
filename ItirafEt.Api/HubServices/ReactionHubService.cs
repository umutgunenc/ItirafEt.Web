using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;
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

        public async Task PostLikedOrDislikedAnonymousAsync(int postId, int? oldReactionTypeId, int newReactionTypeId, Guid UserId)
        {
            await _hubContext.Clients
                .Group($"post-{postId}")
                .SendAsync("PostLikedOrDislikedAnonymousAsync", oldReactionTypeId, newReactionTypeId, UserId);
        }

    }
}
