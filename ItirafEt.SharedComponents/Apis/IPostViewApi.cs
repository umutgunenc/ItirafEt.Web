using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    public interface IPostViewApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/readPost")]
        Task<ApiResponses> ReadPostAsync(int postId, Guid? userId);


        [Headers("Authorization: Bearer")]
        [Get("/api/getPostViewers")]
        Task<ApiResponses<List<PostViewersViewModel>>> GetPostsViewersAsync(int postId);


        //[Headers("Authorization: Bearer")]
        [Get("/api/getPostViewCount")]
        Task<ApiResponses<int>> GetPostViewCountAsync(int postId);
    }
}
