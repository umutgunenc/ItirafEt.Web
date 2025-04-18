using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    public interface IReactionApi
    {
        [Get("/api/getPostReaction/")]
        Task<List<ReactionDto>?> GetPostReactionsAsync(int postId);

        [Headers("Authorization: Bearer")]
        [Post("/api/likePost/")]
        Task<Shared.DTOs.ApiResponse<ReactionDto>> LikePostAsync(int postId, Guid UserId);

        [Headers("Authorization: Bearer")]
        [Post("/api/dislikePost/")]
        Task<Shared.DTOs.ApiResponse<ReactionDto>> DislikePostAsync(int postId, Guid UserId);
    }
}
