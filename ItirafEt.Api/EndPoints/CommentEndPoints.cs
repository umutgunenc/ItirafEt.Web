using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Services;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.EndPoints
{
    public static class CommentEndPoints
    {
        public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/getPostComments", async (CommentService commentService, int postId) =>
                Results.Ok(await commentService.GetPostCommentsAsync(postId)));

            app.MapPost("/api/addComment", async (int PostId,CommentsDto dto, Guid userId, HttpContext context, CommentService commentService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                dto.IpAddress = ipAddress;
                dto.DeviceInfo = userAgent;

                return Results.Ok(await commentService.AddCommentAsync(PostId, userId,dto ));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/addCommentRepyl", async (int postId, int commentId, Guid UserId, CommentsDto replyDto, HttpContext context, CommentService commentService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                replyDto.IpAddress = ipAddress;
                replyDto.DeviceInfo = userAgent;

                return Results.Ok(await commentService.AddCommentReplyAsync(postId, commentId, UserId, replyDto));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            return app;
        }
    }
}
