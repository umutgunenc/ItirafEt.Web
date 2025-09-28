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
            })
            .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapGet("/api/getPostById", async (int postId, PostService postService) =>
            {
                return Results.Ok(await postService.GetPostByIdAsync(postId));
            });


            app.MapGet("/api/getPostInformationById", async (int postId, PostService postService) =>
            {
                return Results.Ok(await postService.GetPostInformationByIdAsync(postId));
            });


            app.MapGet("/api/getUserPost", async (Guid userId, int size, int pageNo, PostService postService) =>
            {
                return Results.Ok(await postService.GetUserPostsAsync(userId, size, pageNo));
            })
            .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/hidePost", async (int postId, Guid userId, PostService postService) =>
            {
                return Results.Ok(await postService.HidePostAsync(postId, userId));
            })
            .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/showPost", async (int postId, Guid userId, PostService postService) =>
            {
                return Results.Ok(await postService.ShowPostAsync(postId, userId));
            })
            .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/canUserEditPost", async (int postId, Guid userId, PostService postService) =>
            {
                return Results.Ok(await postService.CanUserEditPostAsync(postId, userId));
            })
            .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));



            app.MapPost("/api/editPost", async (EditPostViewModel model, HttpContext context, PostService postService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                model.IpAddress = ipAddress;
                model.DeviceInfo = userAgent;

                return Results.Ok(await postService.EditPostAsync(model));
            })
.RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            return app;
        }
    }
}

