using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    [Headers("Authorization: Bearer")]

    public interface ICategoryApi
    {
        [Post("/api/createcategory")]
        Task<ApiResponses> CreateCategoryAsync(CategoryViewModel model);
        [Post("/api/editcategory")]
        Task<ApiResponses> EditCategoryAsync(CategoryViewModel model);
        [Get("/api/getAllCategories")]
        Task<ApiResponses<List<CategoryViewModel>>> GetAllCategoriesAsync();

        [Get("/api/getCategoryName")]
        Task<ApiResponses<string>> GetCategoryNameAsync(int categoryId);

        [Get("/api/getAllActiveCategories")]
        Task<ApiResponses<List<CategoryViewModel>>> GetAllActiveCategoriesAsycn();

        [Get("/api/getCategoryPostsOrderByDate")]
        Task<ApiResponses<List<PostInfoViewModel>>> GetCategoryPostsOrderByDateAsync([Query] int categoryId,[Query] int pageNo,[Query] int pageSize);

        [Get("/api/getCategoryPostsOrderByViewCount")]
        Task<ApiResponses<List<PostInfoViewModel>>> GetCategoryPostsOrderByViewCountAsync([Query] int categoryId,[Query] int pageNo,[Query] int pageSize);

        [Get("/api/getCategoryPostsOrderByLikeCount")]
        Task<ApiResponses<List<PostInfoViewModel>>> GetCategoryPostsOrderByLikeCountAsync([Query] int categoryId, [Query] int pageNo, [Query] int pageSize);


        [Get("/api/getTopFiveCategoryInfo")]
        Task<ApiResponses<List<CategoryButtonInfoViewModel>>> GetTopFiveCategoryInfoAsync();
    }
}
