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

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleenum.SuperAdmin), nameof(UserRoleenum.Admin), nameof(UserRoleenum.Moderator), nameof(UserRoleenum.SuperUser), nameof(UserRoleenum.User)));

            return app;
        }
    }
}
