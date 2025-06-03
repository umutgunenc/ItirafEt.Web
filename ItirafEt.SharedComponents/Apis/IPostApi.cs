using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{

    public interface IPostApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/createPost")]
        Task<ApiResponses> CreatePostAsync(PostViewModel dto,Guid UserId);

        [Headers("Authorization: Bearer")]
        [Get("/api/getCreatedPost/")]
        Task<ApiResponses<int>> GetCreatedPostIdAsync(Guid UserId);

        [Get("/api/getPostById/")]
        Task<ApiResponses<PostViewModel>> GetPostByIdAsync(int postId);

    }
}
