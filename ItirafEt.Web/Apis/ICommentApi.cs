using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    public interface ICommentApi
    {
        [Get("/api/getPostComments/")]
        Task<List<CommentsDto>?> GetPostCommentsAsync(int postId);
    }
}
