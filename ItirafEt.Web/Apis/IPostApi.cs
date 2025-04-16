using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    [Headers("Authorization: Bearer")]

    public interface IPostApi
    {
        [Post("/api/createPost")]
        Task<ApiResponse> CreatePostAsync(PostDto dto,Guid UserId);
        [Get("/api/getCreatedPost/")]
        Task<ApiResponse> GetCreatedPostAsync(Guid UserId);

    }
}
