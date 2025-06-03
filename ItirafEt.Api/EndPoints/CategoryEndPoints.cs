using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;

namespace ItirafEt.Api.EndPoints
{
    public static class CategoryEndPoints
    {
        public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/getAllCategories", async (CategoryService categoryServices) =>
                Results.Ok(await categoryServices.GetAllCategoriesAsync()))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));

            app.MapGet("/api/getAllActiveCategories", async (CategoryService categoryServices) =>
                Results.Ok(await categoryServices.GetAllActiveCategoriesAsycn()));

            app.MapGet("/api/getCategoryName", async (CategoryService categoryServices,int  categoryId) =>
                Results.Ok(await categoryServices.GetCategoryNameAsync(categoryId)));

            app.MapPost("/api/createcategory", async (CategoryViewModel model, CategoryService categoryServices) =>
                Results.Ok(await categoryServices.CreateCategoryAsync(model)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));

            app.MapPost("/api/editcategory", async (CategoryViewModel model, CategoryService categoryServices) =>
                Results.Ok(await categoryServices.EditCategoryAsync(model)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));

            app.MapGet("/api/getCategoryPostsOrderByDate", async (CategoryService categoryServices, int categoryId,int pageNo,int pageSize) =>
                Results.Ok(await categoryServices.GetCategoryPostsOrderByCreatedDateAsync(categoryId, pageNo, pageSize))); 
            
            app.MapGet("/api/getCategoryPostsOrderByViewCount", async (CategoryService categoryServices, int categoryId,int pageNo,int pageSize) =>
                Results.Ok(await categoryServices.GetCategoryPostsOrderByViewCountAsync(categoryId, pageNo, pageSize)));

            app.MapGet("/api/getCategoryPostsOrderByLikeCount", async (CategoryService categoryServices, int categoryId, int pageNo, int pageSize) =>
                Results.Ok(await categoryServices.GetCategoryPostsOrderByLikeCountAsync(categoryId, pageNo, pageSize)));

            return app;
        }
    }
}
