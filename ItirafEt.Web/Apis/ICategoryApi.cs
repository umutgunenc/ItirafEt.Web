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
        [Get("/api/getallactivecategory")]
        Task<ApiResponses<List<CategoryDto>>> GetAllActiveCategoryAsycn();
    }
}
