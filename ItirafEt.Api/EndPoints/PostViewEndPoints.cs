using ItirafEt.Api.Services;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.EndPoints
{
    public static class PostViewEndPoints
    {
        public static IEndpointRouteBuilder MapPostViewEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapPost("/api/readPost", async (int postId, Guid? UserId, PostViewService postReadService) =>
                Results.Ok(await postReadService.ReadPostAsync(postId, UserId)));
                    //.RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapGet("/api/getPostViewers", async (int postId, PostViewService postReadService) =>
                Results.Ok(await postReadService.GetPostsViewersAsync(postId)));
                    //.RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser)));

            //app.MapGet("/api/getUserViewedPostInfo", async (Guid userId, PostViewService postReadService) =>
            //    Results.Ok(await postReadService.GetUserViewedPostInfoAsync(userId)))
            //        .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser)));

            //app.MapGet("/api/getPostReadCount", async (PostViewService postReadService, int postId) =>
            //    Results.Ok(await postReadService.GetPostReadCountAsync(postId)));

            //app.MapGet("/api/didUserReadPost", async (int postId, Guid userId, PostViewService postReadService) =>
            //    Results.Ok(await postReadService.DidUserReadPostAsync(postId, userId)))
            //        .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            return app;
        }
    }
}
