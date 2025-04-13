using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    [Headers("Authorization: Bearer")]

    public interface ICategoryApi
    {
        [Post("/api/createcategory")]
        Task<ApiResponse> CreateCategoryAsync(CategoryDto dto);
        [Post("/api/editcategory")]
        Task<ApiResponse> EditCategoryAsync(CategoryDto dto);
        [Get("/api/getcategory")]
        Task<List<CategoryDto>> GetCategoryAsync();
        [Get("/api/getallactivecategory")]
        Task<List<CategoryDto>> GetAllActiveCategoryAsycn();
    }
}
