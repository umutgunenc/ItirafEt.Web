using ItirafEt.Api.Services;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.Builder;

namespace ItirafEt.Api.EndPoints
{
    public static class ReactionEndPoints
    {
        public static IEndpointRouteBuilder MapReactionEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/getPostReaction", async (ReactionService reactionService, int postId) =>
                Results.Ok(await reactionService.GetPostReactionsAsync(postId)));

            app.MapPost("/api/likePost", async (int postId,Guid UserId, ReactionService reactionService) =>
                Results.Ok(await reactionService.LikePostAsync(postId, UserId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser),nameof(UserRoleEnum.User)));

            app.MapPost("/api/dislikePost", async (int postId, Guid UserId, ReactionService reactionService) =>
                Results.Ok(await reactionService.DislikePostAsync(postId, UserId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            return app;
        }
    }
}
