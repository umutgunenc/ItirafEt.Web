using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    public interface IReactionApi
    {
        [Get("/api/getPostReaction/")]
        Task<ApiResponses<List<ReactionDto>>> GetPostReactionsAsync(int postId);

        [Headers("Authorization: Bearer")]
        [Post("/api/likePost/")]
        Task<ApiResponses> LikePostAsync(int postId, Guid UserId);

        [Headers("Authorization: Bearer")]
        [Post("/api/dislikePost/")]
        Task<ApiResponses> DislikePostAsync(int postId, Guid UserId);
    }
}
