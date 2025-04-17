using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.EndPoints
{
    public static class CommentEndPoints
    {
        public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/getPostComments", async (CommentService commentService, int postId) =>
                Results.Ok(await commentService.GetPostCommentsAsync(postId)));



            return app;
        }
    }
}
