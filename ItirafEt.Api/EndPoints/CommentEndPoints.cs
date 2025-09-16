using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;

namespace ItirafEt.Api.EndPoints
{
    public static class CommentEndPoints
    {
        public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/getPostComments", async (CommentService commentService, int postId) =>
                Results.Ok(await commentService.GetPostCommentsAsync(postId)));
                    //.RequireCors("AllowSpecificOrigin");


            app.MapPost("/api/addComment", async (int PostId, CommentsViewModel model, Guid userId, HttpContext context, CommentService commentService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                model.IpAddress = ipAddress;
                model.DeviceInfo = userAgent;

                return Results.Ok(await commentService.AddCommentAsync(PostId, userId, model));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));
                    //.RequireCors("AllowSpecificOrigin");



            app.MapPost("/api/addCommentRepyl", async (int postId, int commentId, Guid userId, CommentsViewModel model, HttpContext context, CommentService commentService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                model.IpAddress = ipAddress;
                model.DeviceInfo = userAgent;

                return Results.Ok(await commentService.AddCommentReplyAsync(postId, commentId, userId, model));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));
                    //.RequireCors("AllowSpecificOrigin");

            app.MapPost("/api/deleteComment", async (CommentsViewModel model, Guid userId, HttpContext context, CommentService commentService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                model.IpAddress = ipAddress;
                model.DeviceInfo = userAgent;

                return Results.Ok(await commentService.DeleteCommentAsync(model, userId));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));
                    //.RequireCors("AllowSpecificOrigin");

            app.MapPost("/api/editComment", async (CommentsViewModel model, Guid userId, HttpContext context, CommentService commentService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                model.IpAddress = ipAddress;
                model.DeviceInfo = userAgent;

                return Results.Ok(await commentService.EditCommentAsync(model, userId));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));
                    //.RequireCors("AllowSpecificOrigin");


            return app;

        }
    }
}
