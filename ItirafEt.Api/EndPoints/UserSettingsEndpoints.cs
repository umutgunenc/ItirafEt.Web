using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;

namespace ItirafEt.Api.EndPoints
{
    public static class UserSettingsEndpoints
    {
        public static IEndpointRouteBuilder MapUserSettingsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/getUserSettingsInfo", async (UserSettingService userSettingService, Guid userId) =>
               Results.Ok(await userSettingService.GetUserInfoAsync(userId)))
                   .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/changeUserSettingsInfo", async (UserSettingService userSettingService, UserSettingsInfoViewModel model, Guid userId) =>
               Results.Ok(await userSettingService.ChangeUserInfoAsync(userId, model)))
                   .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/changeUserPassword", async (UserSettingService userSettingService, UserSettingsChangePaswordViewModel model, Guid userId) =>
               Results.Ok(await userSettingService.ChangeUserPasswordAsync(userId, model)))
                   .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            return app;
        }
    }
}
