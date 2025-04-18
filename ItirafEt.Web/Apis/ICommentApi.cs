using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    public interface ICommentApi
    {
        [Get("/api/getPostComments/")]
        Task<List<CommentsDto>?> GetPostCommentsAsync(int postId);

        [Headers("Authorization: Bearer")]
        [Post("/api/addComment/")]
        Task<ApiResponse> AddCommentAsync(int postId,Guid UserId, CommentsDto dto);
    }
}
