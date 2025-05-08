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

        public async Task<ApiResponses<List<ReactionDto>>> GetPostReactionsAsync(int postId)
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

            //if (postReactions == null || postReactions.Count == 0)
            //    return ApiResponses<List<ReactionDto>>.Fail("");

            return ApiResponses<List<ReactionDto>>.Success(postReactions);

        }
        public async Task<ApiResponses<ReactionDto>> LikePostAsync(int postId, Guid UserId)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .Include(p => p.PostReactions)
                .FirstOrDefaultAsync(c => c.Id == postId && c.IsActive);
            if (post == null)
                return ApiResponses<ReactionDto>.Fail("Gönderi bulunamadı.");

            var reaction = await _context.PostReaction
                .FirstOrDefaultAsync(c => c.PostId == postId && c.ReactingUserId == UserId);

            if (reaction != null)
            {
                if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Like)
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Cancelled;
                else if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Like;
                else
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Like;

                reaction.CreatedDate = DateTime.Now;
                _context.PostReaction.Update(reaction);

                var reactitonDto = ReactionToReactionDto(reaction);

                await _context.SaveChangesAsync();
                await _hubContext.Clients
                    .Group($"post-{postId}")
                    .SendAsync("PostLikedOrDisliked", reactitonDto, true);
                return ApiResponses<ReactionDto>.Success(reactitonDto, true);
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

                var reactionDto = ReactionToReactionDto(newReaction);

                await _context.SaveChangesAsync();
                await _hubContext.Clients
                    .Group($"post-{postId}")
                    .SendAsync("PostLikedOrDisliked", reactionDto, false);
                return ApiResponses<ReactionDto>.Success(reactionDto, false);
            }

        }
        public async Task<ApiResponses<ReactionDto>> DislikePostAsync(int postId, Guid UserId)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .Include(p => p.PostReactions)
                .FirstOrDefaultAsync(c => c.Id == postId && c.IsActive);
            if (post == null)
                return ApiResponses<ReactionDto>.Fail("Gönderi bulunamadı.");

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

                await _context.SaveChangesAsync();

                var reactionDto = ReactionToReactionDto(reaction);
                await _hubContext.Clients
                    .Group($"post-{postId}")
                    .SendAsync("PostLikedOrDisliked", reactionDto, true);
                return ApiResponses<ReactionDto>.Success(reactionDto, true);
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



                await _context.SaveChangesAsync();

                var reactionDto = ReactionToReactionDto(newReaction);
                await _hubContext.Clients
                    .Group($"post-{postId}")
                    .SendAsync("PostLikedOrDisliked", reactionDto, false);
                return ApiResponses<ReactionDto>.Success(reactionDto, false);
            }

        }
        private ReactionDto ReactionToReactionDto(PostReaction reaction)
        {
            return new ReactionDto
            {
                PostId = reaction.PostId,
                ReactingUserId = reaction.ReactingUserId,
                ReactionTypeId = reaction.ReactionTypeId,
            };
        }
    }
}
