using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    public interface IReactionApi
    {
        [Headers("Authorization: Bearer")]
        [Get("/api/getPostReaction/")]
        Task<ApiResponses<List<ReactionViewModel>>> GetPostReactionsAsync(int postId);

        [Headers("Authorization: Bearer")]
        [Post("/api/likePost/")]
        Task<ApiResponses> LikePostAsync(int postId, Guid UserId);

        [Headers("Authorization: Bearer")]
        [Post("/api/dislikePost/")]
        Task<ApiResponses> DislikePostAsync(int postId, Guid UserId);

        [Get("/api/getPostLikeCount/")]
        Task<ApiResponses<int>> GetPostLikeCountAsync(int postId);

        [Get("/api/getPostDislikeCount/")]
        Task<ApiResponses<int>> GetPostDislikeCountAsync(int postId);

        [Headers("Authorization: Bearer")]
        [Get("/api/getUserReactionTypeId/")]
        Task<ApiResponses<int?>> GetUserReactionTypeIdAsync(int postId, Guid? UserId);
    }
}
