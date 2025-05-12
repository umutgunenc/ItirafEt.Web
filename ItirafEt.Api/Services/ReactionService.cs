using System.Net.NetworkInformation;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class ReactionService
    {
        private readonly dbContext _context;
        private readonly ReactionHubService _reactionHubService;


        public ReactionService(dbContext context, ReactionHubService reactionHubService)
        {
            _context = context;
            _reactionHubService = reactionHubService;
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

            return ApiResponses<List<ReactionDto>>.Success(postReactions);

        }
        public async Task<ApiResponses<ReactionDto>> LikePostAsync(int postId, Guid UserId)
        {
            var post = await GetPostAsync(postId);
            if (post == null)
                return ApiResponses<ReactionDto>.Fail("Gönderi bulunamadı.");

            var reaction = await GetPostReactionAsync(postId, UserId);

            var likeCount = post.PostReactions.Count(c => c.ReactionTypeId == (int)ReactionTypeEnum.Like);
            if (reaction != null)
            {
                if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Like)
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Cancelled;
                    likeCount--;
                }
                else if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Like;
                    likeCount++;
                }
                else
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Like;
                    likeCount++;
                }

                reaction.CreatedDate = DateTime.Now;
                _context.PostReaction.Update(reaction);
                await _context.SaveChangesAsync();

                var reactionDto = ReactionToReactionDto(reaction);

                await _reactionHubService.PostLikedOrDislikedAsync(postId, reactionDto, true);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);

                return ApiResponses<ReactionDto>.Success(reactionDto, true);

            }
            else
            {
                reaction = new PostReaction
                {
                    PostId = postId,
                    ReactingUserId = UserId,
                    ReactionTypeId = (int)ReactionTypeEnum.Like,
                    CreatedDate = DateTime.Now
                };
                _context.PostReaction.Add(reaction);
                likeCount++;

                await _context.SaveChangesAsync();
                reaction.ReactingUser = await GetReactingUser(UserId);

                var reactionDto = ReactionToReactionDto(reaction);

                await _reactionHubService.PostLikedOrDislikedAsync(postId, reactionDto, false);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);

                return ApiResponses<ReactionDto>.Success(reactionDto, false);

            }


        }
        public async Task<ApiResponses<ReactionDto>> DislikePostAsync(int postId, Guid UserId)
        {
            var post = await GetPostAsync(postId);
            if (post == null)
                return ApiResponses<ReactionDto>.Fail("Gönderi bulunamadı.");

            var reaction = await GetPostReactionAsync(postId, UserId);

            var likeCount = post.PostReactions.Count(c => c.ReactionTypeId == (int)ReactionTypeEnum.Like);

            if (reaction != null)
            {
                if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Cancelled;
                }
                else if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Like)
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Dislike;
                    likeCount--;
                }
                else
                {
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Dislike;
                }

                reaction.CreatedDate = DateTime.Now;

                _context.PostReaction.Update(reaction);
                await _context.SaveChangesAsync();
                var reactionDto = ReactionToReactionDto(reaction);
                await _reactionHubService.PostLikedOrDislikedAsync(postId, reactionDto, true);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);

                return ApiResponses<ReactionDto>.Success(reactionDto, true);

            }
            else
            {
                reaction = new PostReaction
                {
                    PostId = postId,
                    ReactingUserId = UserId,
                    ReactionTypeId = (int)ReactionTypeEnum.Dislike,
                    CreatedDate = DateTime.Now
                };
                _context.PostReaction.Add(reaction);

                await _context.SaveChangesAsync();

                reaction.ReactingUser = await GetReactingUser(UserId);
                var reactionDto = ReactionToReactionDto(reaction);
                await _reactionHubService.PostLikedOrDislikedAsync(postId, reactionDto, false);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);

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
                ReactingUserUserName = reaction.ReactingUser.UserName,
                CreatedDate = reaction.CreatedDate,

            };
        }
        private async Task<Post?> GetPostAsync(int postId)
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(p => p.PostReactions)
                .FirstOrDefaultAsync(c => c.Id == postId && c.IsActive);
        }
        private async Task<PostReaction?> GetPostReactionAsync(int postId, Guid UserId)
        {
            return await _context.PostReaction
                .Include(c => c.ReactingUser)
                .FirstOrDefaultAsync(c => c.PostId == postId && c.ReactingUserId == UserId);
        }
        private async Task<User> GetReactingUser(Guid UserId)
        {
           return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == UserId);

        }
    }
}
