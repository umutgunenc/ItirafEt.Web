using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;

namespace ItirafEt.Api.EndPoints
{
    public static class PostEndPoints
    {
        public static IEndpointRouteBuilder MapPostEndPoints(this IEndpointRouteBuilder app)
        {

            app.MapPost("/api/createPost", async (PostViewModel model, Guid userId, HttpContext context, PostService postService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                model.IpAddress = ipAddress;
                model.DeviceInfo = userAgent;

                return Results.Ok(await postService.CreatePostAsync(model, userId));
            })
            .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapGet("/api/getCreatedPost", async (Guid userId, PostService postService) =>
            {
                return Results.Ok(await postService.GetCreatedPostIdAsync(userId));
            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapGet("/api/getPostById", async (int postId, PostService postService) =>
            {
                return Results.Ok(await postService.GetPostByIdAsync(postId));
            });

            return app;
        }
    }
}

