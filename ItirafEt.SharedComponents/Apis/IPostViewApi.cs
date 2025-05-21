using ItirafEt.Shared.DTOs;
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
        Task<ApiResponses<List<PostViewersDto>>> GetPostsViewersAsync(int postId);


        //[Headers("Authorization: Bearer")]
        [Get("/api/getPostViewCount")]
        Task<ApiResponses<int>> GetPostViewCountAsync(int postId);

        //[Headers("Authorization: Bearer")]
        //[Get("/api/getUserViewedPostInfo")]
        //Task<ApiResponses<List<UserViewedPostsDto>>> GetUserViewedPostInfoAsync(Guid userId);


        //[Get("/api/getPostReadCount")]
        //Task<ApiResponses<int>> GetPostReadCountAsync(int postId);


        //[Headers("Authorization: Bearer")]
        //[Get("/api/didUserReadPost")]
        //Task<ApiResponses<bool>> DidUserReadPostAsync(int postId, Guid userId);

    }
}
