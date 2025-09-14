using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
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
        public async Task PostLikedOrDislikedAsync(ReactionViewModel reactionModel, bool isUpdated)
        {
            await _hubContext.Clients
                .Group($"post-{reactionModel.PostId}")
                .SendAsync("PostLikedOrDislikedAsync", reactionModel, isUpdated);
        }

        public async Task UpdatePostLikeCountAsync(int categoryId, int postId, int likeCount)
        {
            await _hubContext.Clients
                .Group($"category-{categoryId}")
                .SendAsync("UpdatePostLikeCountAsync", postId, likeCount);
        }

        public async Task PostLikedOrDislikedAnonymousAsync(int postId, int? oldReactionTypeId, int newReactionTypeId)
        {
            await _hubContext.Clients
                .Group($"post-{postId}")
                .SendAsync("PostLikedOrDislikedAnonymousAsync", oldReactionTypeId, newReactionTypeId);
        }

        public async Task CommentLikedOrDislikedAnonymousAsync(int postId, int commentId, int? oldReactionTypeId, int newReactionTypeId)
        {
            await _hubContext.Clients
                .Group($"post-{postId}")
                .SendAsync("CommentLikedOrDislikedAnonymousAsync", commentId, oldReactionTypeId, newReactionTypeId);
        }

        public async Task CommentLikedOrDislikedAsync(ReactionViewModel reactionModel, bool isUpdated)
        {
            await _hubContext.Clients
                .Group($"post-{reactionModel.PostId}")
                .SendAsync("CommentLikedOrDislikedAsync", reactionModel, isUpdated);
        }

    }
}
