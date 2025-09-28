using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.Builder;

namespace ItirafEt.Api.EndPoints
{
    public static class ReactionEndPoints
    {
        public static IEndpointRouteBuilder MapReactionEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/getPostReaction", async (ReactionService reactionService, int postId) =>
                Results.Ok(await reactionService.GetPostReactionsAsync(postId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser)));

            app.MapPost("/api/likePost", async (int postId, Guid userId, ReactionService reactionService) =>
                Results.Ok(await reactionService.LikePostAsync(postId, userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/dislikePost", async (int postId, Guid userId, ReactionService reactionService) =>
                Results.Ok(await reactionService.DislikePostAsync(postId, userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapGet("/api/getPostLikeCount", async (ReactionService reactionService, int postId) =>
                Results.Ok(await reactionService.GetPostLikeCountAsync(postId)));

            app.MapGet("/api/getPostDislikeCount", async (ReactionService reactionService, int postId) =>
                Results.Ok(await reactionService.GetPostDislikeCountAsync(postId)));

            app.MapGet("/api/getUserReactionTypeId", async (ReactionService reactionService, int postId, Guid? userId) =>
                Results.Ok(await reactionService.GetUserReactionTypeIdAsync(postId, userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/dislikeComment", async (int commentId, Guid userId, ReactionService reactionService) =>
                Results.Ok(await reactionService.DislikeCommentAsync(commentId, userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/likeComment", async (int commentId, Guid userId, ReactionService reactionService) =>
                Results.Ok(await reactionService.LikeCommentAsync(commentId, userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            return app;
        }
    }
}
