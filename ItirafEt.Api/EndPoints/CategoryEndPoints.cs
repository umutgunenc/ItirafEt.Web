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
            //.RequireCors("AllowSpecificOrigin");

            app.MapGet("/api/getAllActiveCategories", async (CategoryService categoryServices) =>
                Results.Ok(await categoryServices.GetAllActiveCategoriesAsycn()));
            //.RequireCors("AllowSpecificOrigin");


            app.MapGet("/api/getCategoryName", async (CategoryService categoryServices, int categoryId) =>
                Results.Ok(await categoryServices.GetCategoryNameAsync(categoryId)));
            //.RequireCors("AllowSpecificOrigin");


            app.MapPost("/api/createcategory", async (CategoryViewModel model, CategoryService categoryServices) =>
                Results.Ok(await categoryServices.CreateCategoryAsync(model)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));
            //.RequireCors("AllowSpecificOrigin");


            app.MapPost("/api/editcategory", async (CategoryViewModel model, CategoryService categoryServices) =>
                Results.Ok(await categoryServices.EditCategoryAsync(model)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));
            //.RequireCors("AllowSpecificOrigin");

            app.MapGet("/api/getCategoryPostsOrderByDate", async (CategoryService categoryServices, int categoryId, int pageNo, int pageSize) =>
                Results.Ok(await categoryServices.GetCategoryPostsOrderByCreatedDateAsync(categoryId, pageNo, pageSize)));
            //.RequireCors("AllowSpecificOrigin");


            app.MapGet("/api/getCategoryPostsOrderByViewCount", async (CategoryService categoryServices, int categoryId, int pageNo, int pageSize) =>
                Results.Ok(await categoryServices.GetCategoryPostsOrderByViewCountAsync(categoryId, pageNo, pageSize)));
            //.RequireCors("AllowSpecificOrigin");


            app.MapGet("/api/getCategoryPostsOrderByLikeCount", async (CategoryService categoryServices, int categoryId, int pageNo, int pageSize) =>
                Results.Ok(await categoryServices.GetCategoryPostsOrderByLikeCountAsync(categoryId, pageNo, pageSize)));
            //.RequireCors("AllowSpecificOrigin");


            return app;
        }
    }
}
