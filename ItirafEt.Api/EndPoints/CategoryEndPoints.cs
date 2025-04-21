using ItirafEt.Api.Services;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.EndPoints
{
    public static class CategoryEndPoints
    {
        public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/getcategory", async (CategoryService categoryServices) =>
                Results.Ok(await categoryServices.GetAllCategoriesAsync()))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));

            app.MapGet("/api/getallactivecategory", async (CategoryService categoryServices) =>
                Results.Ok(await categoryServices.GetAllActiveCategoriesAsycn()));

            app.MapPost("/api/createcategory", async (CategoryDto dto, CategoryService categoryServices) =>
                Results.Ok(await categoryServices.CreateCategoryAsync(dto)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));

            app.MapPost("/api/editcategory", async (CategoryDto dto, CategoryService categoryServices) =>
                Results.Ok(await categoryServices.EditCategoryAsync(dto)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));

            return app;
        }
    }
}
