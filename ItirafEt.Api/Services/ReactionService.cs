using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class ReactionService
    {
        private readonly dbContext _context;
        private readonly IHubContext<ReactionHub> _hubContext;


        public ReactionService(dbContext context, IHubContext<ReactionHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<List<ReactionDto>?> GetPostReactionsAsync(int postId)
        {
            var postReactions = await _context.PostReaction
                .AsNoTracking()
                .Include(pr => pr.ReactionType)
                .Include(pr => pr.ReactingUser)
                .AsNoTracking()
                .Where(pr => pr.PostId == postId)
                .Select(pr => new ReactionDto
                {
                    Id = pr.Id,
                    PostId = pr.PostId,
                    ReactionTypeId = pr.ReactionTypeId,
                    ReactingUserId = pr.ReactingUserId,
                    ReactingUserUserName = pr.ReactingUser.UserName,
                    CreatedDate = pr.CreatedDate,
                    ReactionTypeName = pr.ReactionType.Name
                })
                .ToListAsync();

            return postReactions;
        }

        public async Task<ApiResponse<ReactionDto>> LikePostAsync(int postId, Guid UserId)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .Include(p => p.PostReactions)
                .FirstOrDefaultAsync(c => c.Id == postId && c.IsActive);
            if (post == null)
                return ApiResponse<ReactionDto>.Fail("Gönderi bulunamadı.");

            var reaction = await _context.PostReaction
                .FirstOrDefaultAsync(c => c.PostId == postId && c.ReactingUserId == UserId);

            if (reaction != null)
            {
                if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Like)
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Cancelled;
                else if(reaction.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Like;
                else 
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Like;

                reaction.CreatedDate = DateTime.Now;
                _context.PostReaction.Update(reaction);

                var reactitonDto = new ReactionDto
                {
                    PostId = reaction.PostId,
                    ReactingUserId = reaction.ReactingUserId,
                    ReactionTypeId = reaction.ReactionTypeId,
                };
                await _hubContext.Clients
                    .Group($"post-{postId}")
                    .SendAsync("NotifyPostLikedOrDisliked", postId, reactitonDto, true);
                await _context.SaveChangesAsync();
                return ApiResponse<ReactionDto>.Success(reactitonDto, true);
            }
            else
            {
                var newReaction = new PostReaction
                {
                    PostId = postId,
                    ReactingUserId = UserId,
                    ReactionTypeId = (int)ReactionTypeEnum.Like,
                    CreatedDate = DateTime.Now
                };
                _context.PostReaction.Add(newReaction);

                var reactionDto = new ReactionDto
                {
                    PostId = newReaction.PostId,
                    ReactingUserId = newReaction.ReactingUserId,
                    ReactionTypeId = newReaction.ReactionTypeId,
                };
                await _hubContext.Clients
                    .Group($"post-{postId}")
                    .SendAsync("NotifyPostLikedOrDisliked", postId, reactionDto, false);
                await _context.SaveChangesAsync();
                return ApiResponse<ReactionDto>.Success(reactionDto, false);
            }


            //await _hubContext.Clients
            //    .Group($"post-{postId}")
            //    .SendAsync("NotifyPostLikedOrDisliked", postId, newReactionId, oldReactionId);

        }

        public async Task<ApiResponse<ReactionDto>> DislikePostAsync(int postId,Guid UserId)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .Include(p => p.PostReactions)
                .FirstOrDefaultAsync(c => c.Id == postId && c.IsActive);
            if (post == null)
                return ApiResponse<ReactionDto>.Fail("Gönderi bulunamadı.");

            var reaction = await _context.PostReaction
                .FirstOrDefaultAsync(c => c.PostId == postId && c.ReactingUserId == UserId);

            if (reaction != null)
            {
                if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Cancelled;
                else if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Like)
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Dislike;
                else
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Dislike;

                reaction.CreatedDate = DateTime.Now;

                _context.PostReaction.Update(reaction);

                await _hubContext.Clients
                    .Group($"post-{postId}")
                    .SendAsync("NotifyPostLikedOrDisliked", postId, reaction, true);

                await _context.SaveChangesAsync();
                var reactionDto = new ReactionDto
                {
                    PostId = reaction.PostId,
                    ReactingUserId = reaction.ReactingUserId,
                    ReactionTypeId = reaction.ReactionTypeId,
                };
                return ApiResponse<ReactionDto>.Success(reactionDto, true);
            }
            else
            {
                var newReaction = new PostReaction
                {
                    PostId = postId,
                    ReactingUserId = UserId,
                    ReactionTypeId = (int)ReactionTypeEnum.Dislike,
                    CreatedDate = DateTime.Now
                };
                _context.PostReaction.Add(newReaction);

                await _hubContext.Clients
                    .Group($"post-{postId}")
                    .SendAsync("NotifyPostLikedOrDisliked", postId, newReaction, false);

                await _context.SaveChangesAsync();
                var reactionDto = new ReactionDto
                {
                    PostId = newReaction.PostId,
                    ReactingUserId = newReaction.ReactingUserId,
                    ReactionTypeId = newReaction.ReactionTypeId,
                };
                return ApiResponse<ReactionDto>.Success(reactionDto, false);
            }

        }
    }
}
