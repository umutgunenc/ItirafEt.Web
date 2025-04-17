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
                Results.Ok(await categoryServices.GetCategoryAsync()))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleenum.SuperAdmin), nameof(UserRoleenum.Admin)));

            app.MapGet("/api/getallactivecategory", async (CategoryService categoryServices) =>
                Results.Ok(await categoryServices.GetAllActiveCategoryAsycn()));

            app.MapPost("/api/createcategory", async (CategoryDto dto, CategoryService categoryServices) =>
                Results.Ok(await categoryServices.CreateCategoryAsync(dto)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleenum.SuperAdmin), nameof(UserRoleenum.Admin)));

            app.MapPost("/api/editcategory", async (CategoryDto dto, CategoryService categoryServices) =>
                Results.Ok(await categoryServices.EditCategoryAsync(dto)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleenum.SuperAdmin), nameof(UserRoleenum.Admin)));

            return app;
        }
    }
}
