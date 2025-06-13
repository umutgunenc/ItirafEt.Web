using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{

    public interface IPostApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/createPost")]
        Task<ApiResponses> CreatePostAsync(PostViewModel model,Guid UserId);

        [Headers("Authorization: Bearer")]
        [Get("/api/getCreatedPost/")]
        Task<ApiResponses<int>> GetCreatedPostIdAsync(Guid UserId);

        [Get("/api/getPostById/")]
        Task<ApiResponses<PostViewModel>> GetPostByIdAsync(int postId);

        [Headers("Authorization: Bearer")]
        [Get("/api/getUserPost/")]
        Task<ApiResponses<UserPostsViewModel>> GetUserPostsAsync(Guid UserId,int size, int PageNo);


        [Headers("Authorization: Bearer")]
        [Post("/api/hidePost")]
        Task<ApiResponses> HidePostAsync(int postId, Guid UserId);

        [Headers("Authorization: Bearer")]
        [Post("/api/showPost")]
        Task<ApiResponses> ShowPostAsync(int postId, Guid UserId);
    }
}
