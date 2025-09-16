using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.EndPoints
{
    public static class UserProfileEndPoints
    {
        public static IEndpointRouteBuilder MapUserProfileEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/getUserProfile", async (UserProfileService profileService, Guid userId) =>
                Results.Ok(await profileService.GetUserProfileAsync(userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));
            //.RequireCors("AllowSpecificOrigin");


            app.MapGet("/api/getUserPostsDateOrdered", async (UserProfileService profileService, Guid userId, int size, int page) =>
                Results.Ok(await profileService.GetUserPostsDateOrderedAsync(userId, size, page)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));
            //.RequireCors("AllowSpecificOrigin");


            app.MapGet("/api/getUserPostsLikeOrdered", async (UserProfileService profileService, Guid userId, int size, int page) =>
                Results.Ok(await profileService.GetUserPostsLikeCountOrderedAsync(userId, size, page)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));
            //.RequireCors("AllowSpecificOrigin");


            app.MapGet("/api/getUserPostsReadOrdered", async (UserProfileService profileService, Guid userId, int size, int page) =>
                Results.Ok(await profileService.GetUserPostsReadCountOrderedAsync(userId, size, page)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));
                    //.RequireCors("AllowSpecificOrigin");



            return app;
        }

    }
}
