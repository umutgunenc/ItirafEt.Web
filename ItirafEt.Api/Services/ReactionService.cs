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

        public async Task<ApiResponse> LikePostAsync(int postId, Guid UserId)
        {
            var post = await _context.Posts
                .Include(p => p.PostReactions)
                .FirstOrDefaultAsync(c => c.Id == postId && c.IsActive);
            if (post == null)
                return ApiResponse.Fail("Gönderi bulunamadı.");

            var reaction = await _context.PostReaction
                .FirstOrDefaultAsync(c => c.PostId == postId && c.ReactingUserId == UserId);

            if (reaction != null)
            {
                if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Like)
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Cancelled;
                    post.LikeCount--;

                }
                else if(reaction.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Like;
                    post.LikeCount++;
                    post.DislikeCount--;
                }
                else 
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Like;
                    post.LikeCount++;
                }

                reaction.CreatedDate = DateTime.Now;
                _context.PostReaction.Update(reaction);

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
                post.LikeCount++;
                _context.PostReaction.Add(newReaction);
            }
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("LikedOrDisliked");
            return ApiResponse.Success();
        }

        public async Task<ApiResponse> DislikePostAsync(int postId,Guid UserId)
        {
            var post = await _context.Posts
                .Include(p => p.PostReactions)
                .FirstOrDefaultAsync(c => c.Id == postId && c.IsActive);
            if (post == null)
                return ApiResponse.Fail("Gönderi bulunamadı.");

            var reaction = await _context.PostReaction
                .FirstOrDefaultAsync(c => c.PostId == postId && c.ReactingUserId == UserId);

            if (reaction != null)
            {
                if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Cancelled;
                    post.DislikeCount--;

                }
                else if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Like)
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Dislike;
                    post.LikeCount--;
                    post.DislikeCount++;
                }
                else
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Dislike;
                    post.DislikeCount++;
                }

                reaction.CreatedDate = DateTime.Now;
                _context.PostReaction.Update(reaction);

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
                post.DislikeCount++;
                _context.PostReaction.Add(newReaction);
            }
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("LikedOrDisliked");
            return ApiResponse.Success();
        }
    }
}
