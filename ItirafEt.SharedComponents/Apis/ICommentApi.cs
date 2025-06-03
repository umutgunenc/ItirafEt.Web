using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    public interface ICommentApi
    {
        [Get("/api/getPostComments/")]
        Task<ApiResponses<List<CommentsViewModel>>> GetPostCommentsAsync(int postId);

        [Headers("Authorization: Bearer")]
        [Post("/api/addComment/")]
        Task<ApiResponses> AddCommentAsync(int postId,Guid UserId, CommentsViewModel model);

        [Headers("Authorization: Bearer")]
        [Post("/api/addCommentRepyl/")]
        Task<ApiResponses> AddReplyCommentAsync(int postId, int commentId, Guid UserId, CommentsViewModel replyModel);
    }
}
