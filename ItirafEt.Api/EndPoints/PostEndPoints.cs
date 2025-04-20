using ItirafEt.Api.Services;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.EndPoints
{
    public static class PostEndPoints
    {
        public static IEndpointRouteBuilder MapPostEndPoints(this IEndpointRouteBuilder app)
        {

            app.MapPost("/api/createPost", async (PostDto dto, Guid userId, HttpContext context, PostService postService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                dto.IpAddress = ipAddress;
                dto.DeviceInfo = userAgent;

                return Results.Ok(await postService.CreatePostAsync(dto, userId));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleenum.SuperAdmin), nameof(UserRoleenum.Admin), nameof(UserRoleenum.Moderator), nameof(UserRoleenum.SuperUser), nameof(UserRoleenum.User)));

            app.MapGet("/api/getCreatedPost", async (Guid userId, PostService postService) =>
            {
                return Results.Ok(await postService.GetCreatedPostIdAsync(userId));
            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleenum.SuperAdmin), nameof(UserRoleenum.Admin), nameof(UserRoleenum.Moderator), nameof(UserRoleenum.SuperUser), nameof(UserRoleenum.User)));

            app.MapGet("/api/getPostById", async (int postId, PostService postService) =>
            {
                return Results.Ok(await postService.GetPostByIdAsync(postId));
            });

            return app;
        }
    }
}

