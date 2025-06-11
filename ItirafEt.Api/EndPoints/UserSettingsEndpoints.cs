using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.EndPoints
{
    public static class UserSettingsEndpoints
    {
        public static IEndpointRouteBuilder MapUserSettingsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/getUserSettingsInfo", async (UserSettingService userSettingService, Guid userId) =>
               Results.Ok(await userSettingService.GetUserInfoAsync(userId)))
                   .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            return app;
        }


    }
}
