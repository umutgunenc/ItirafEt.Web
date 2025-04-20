using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{

    public interface IPostApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/createPost")]
        Task<ApiResponses> CreatePostAsync(PostDto dto,Guid UserId);

        [Headers("Authorization: Bearer")]
        [Get("/api/getCreatedPost/")]
        Task<ApiResponses<int>> GetCreatedPostIdAsync(Guid UserId);

        [Get("/api/getPostById/")]
        Task<ApiResponses<PostDto>> GetPostByIdAsync(int postId);

        //[Headers("Authorization: Bearer")]
        //[Post("/api/likePost")]
        //Task<ApiResponse> LikePostAsync(int postId, Guid UserId);

        //[Headers("Authorization: Bearer")]
        //[Post("/api/disliktePost")]
        //Task<ApiResponse> DislikePostAsync(int postId, Guid UserId);

    }
}
