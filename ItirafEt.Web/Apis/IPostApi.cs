using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{

    public interface IPostApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/createPost")]
        Task<ApiResponse> CreatePostAsync(PostDto dto,Guid UserId);

        [Headers("Authorization: Bearer")]
        [Get("/api/getCreatedPost/")]
        Task<ApiResponse> GetCreatedPostAsync(Guid UserId);

        [Get("/api/getPostById/")]
        Task<PostDto> GetPostByIdAsync(int postId);

        //[Headers("Authorization: Bearer")]
        //[Post("/api/likePost")]
        //Task<ApiResponse> LikePostAsync(int postId, Guid UserId);

        //[Headers("Authorization: Bearer")]
        //[Post("/api/disliktePost")]
        //Task<ApiResponse> DislikePostAsync(int postId, Guid UserId);

    }
}
