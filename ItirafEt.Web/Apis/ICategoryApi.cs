using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    [Headers("Authorization: Bearer")]

    public interface ICategoryApi
    {
        [Post("/api/createcategory")]
        Task<ApiResponses> CreateCategoryAsync(CategoryDto dto);
        [Post("/api/editcategory")]
        Task<ApiResponses> EditCategoryAsync(CategoryDto dto);
        [Get("/api/getcategory")]
        Task<ApiResponses<List<CategoryDto>>> GetCategoryAsync();

        [Get("/api/getCategoryName")]
        Task<ApiResponses<string>> GetCategoryNameAsync(int categoryId);

        [Get("/api/getallactivecategory")]
        Task<ApiResponses<List<CategoryDto>>> GetAllActiveCategoryAsycn();

        [Get("/api/getCategoryPostsOrderByDate")]
        Task<ApiResponses<List<PostInfoDto>>> GetCategoryPostsOrderByDateAsync([Query] int categoryId,[Query] int pageNo,[Query] int pageSize);

        [Get("/api/getCategoryPostsOrderByViewCount")]
        Task<ApiResponses<List<PostInfoDto>>> GetCategoryPostsOrderByViewCountAsync([Query] int categoryId,[Query] int pageNo,[Query] int pageSize);

        [Get("/api/getCategoryPostsOrderByLikeCount")]
        Task<ApiResponses<List<PostInfoDto>>> GetCategoryPostsOrderByLikeCountAsync([Query] int categoryId, [Query] int pageNo, [Query] int pageSize);
    }
}
