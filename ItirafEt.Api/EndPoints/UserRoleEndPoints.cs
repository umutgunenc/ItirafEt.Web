using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItirafEt.Api.EndPoints
{
    public static class UserRoleEndPoints
    {
        public static IEndpointRouteBuilder MapUserRoleEndPoints(this IEndpointRouteBuilder app)
        {

            app.MapPost("/api/selectUser", async (UserRoleService changeUserRoleService, ChangeUserRoleViewModel model) =>
                Results.Ok(await changeUserRoleService.SelecetUserAsync(model)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin)));

            app.MapPost("/api/changeUserRole", async (UserRoleService changeUserRoleService, ChangeUserRoleViewModel model) =>
                 Results.Ok(await changeUserRoleService.ChangeUserRoleAsync(model)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin)));

            app.MapPost("/api/getUsersWithRoles", async (UserRoleService changeUserRoleService, SelectOptionForUserWithRoleViewModel model) =>
                Results.Ok(await changeUserRoleService.GetAllUsersWithRolesAsync(model)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin)));

            app.MapGet("/api/getAllRoles", async (UserRoleService changeUserRoleService) =>
                Results.Ok(await changeUserRoleService.GetAllRolesAsync()))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin)));

            return app;
        }
    }
}
