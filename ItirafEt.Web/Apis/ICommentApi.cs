using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    public interface ICommentApi
    {
        [Get("/api/getPostComments/")]
        Task<ApiResponses<List<CommentsDto>>> GetPostCommentsAsync(int postId);

        [Headers("Authorization: Bearer")]
        [Post("/api/addComment/")]
        Task<ApiResponses> AddCommentAsync(int postId,Guid UserId, CommentsDto dto);

        [Headers("Authorization: Bearer")]
        [Post("/api/addCommentRepyl/")]
        Task<ApiResponses> AddReplyCommentAsync(int postId, int commentId, Guid UserId, CommentsDto replyDto);
    }
}
