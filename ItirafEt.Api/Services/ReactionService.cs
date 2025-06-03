using System.Net.NetworkInformation;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
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

        public async Task<ApiResponses<List<ReactionViewModel>>> GetPostReactionsAsync(int postId)
        {
            var postReactions = await _context.PostReaction
                .AsNoTracking()
                .Include(pr => pr.ReactionType)
                .Include(pr => pr.ReactingUser)
                .AsNoTracking()
                .Where(pr => pr.PostId == postId)
                .Select(pr => new ReactionViewModel
                {
                    Id = pr.Id,
                    PostId = pr.PostId,
                    ReactionTypeId = pr.ReactionTypeId,
                    ReactingUserId = pr.ReactingUserId,
                    ReactingUserUserName = pr.ReactingUser.UserName,
                    CreatedDate = pr.CreatedDate,
                    ReactingUserAge = DateTime.Now.Year - pr.ReactingUser.BirthDate.Year,
                    ReactingUserGenderId = pr.ReactingUser.GenderId,
                    ReactingUserProfileImageUrl = pr.ReactingUser.ProfilePictureUrl
                })
                .ToListAsync();

            return ApiResponses<List<ReactionViewModel>>.Success(postReactions);

        }
        public async Task<ApiResponses<int>> GetPostLikeCountAsync(int podtId)
        {
            var likeCount = await _context.PostReaction
                .AsNoTracking()
                .Where(p=>p.PostId == podtId && p.ReactionTypeId == (int)ReactionTypeEnum.Like)
                .CountAsync();

            return ApiResponses<int>.Success(likeCount);
        } 
        public async Task<ApiResponses<int>> GetPostDislikeCountAsync(int podtId)
        {
            var dislikeCount = await _context.PostReaction
                .AsNoTracking()
                .Where(p=>p.PostId == podtId && p.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                .CountAsync();

            return ApiResponses<int>.Success(dislikeCount);
        }
        public async Task<ApiResponses> LikePostAsync(int postId, Guid UserId)
        {
            var post = await GetPostAsync(postId);
            if (post == null)
                return ApiResponses.Fail("Gönderi bulunamadı.");

            var reaction = await GetPostReactionAsync(postId, UserId);

            var likeCount = post.PostReactions.Count(c => c.ReactionTypeId == (int)ReactionTypeEnum.Like);
            if (reaction != null)
            {
                var oldReactionTypeId = reaction.ReactionTypeId;
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

                var reactionModel = ReactionToReactionModel(reaction);

                await _reactionHubService.PostLikedOrDislikedAsync(postId, reactionModel, true);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);
                await _reactionHubService.PostLikedOrDislikedAnonymousAsync(postId, oldReactionTypeId, reaction.ReactionTypeId, UserId);

                return ApiResponses.Success();

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

                var reactionDto = ReactionToReactionModel(reaction);

                await _reactionHubService.PostLikedOrDislikedAsync(postId, reactionDto, false);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);
                await _reactionHubService.PostLikedOrDislikedAnonymousAsync(postId, null, reaction.ReactionTypeId, UserId);

                return ApiResponses.Success();

            }


        }
        public async Task<ApiResponses> DislikePostAsync(int postId, Guid UserId)
        {
            var post = await GetPostAsync(postId);
            if (post == null)
                return ApiResponses.Fail("Gönderi bulunamadı.");

            var reaction = await GetPostReactionAsync(postId, UserId);

            var likeCount = post.PostReactions.Count(c => c.ReactionTypeId == (int)ReactionTypeEnum.Like);

            if (reaction != null)
            {
                var oldReactionTypeId = reaction.ReactionTypeId;

                if (reaction.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                    reaction.ReactionTypeId = (int)ReactionTypeEnum.Cancelled;
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
                var reactionDto = ReactionToReactionModel(reaction);
                await _reactionHubService.PostLikedOrDislikedAsync(postId, reactionDto, true);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);
                await _reactionHubService.PostLikedOrDislikedAnonymousAsync(postId, oldReactionTypeId, reaction.ReactionTypeId, UserId);

                return ApiResponses.Success();

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
                var reactionDto = ReactionToReactionModel(reaction);
                await _reactionHubService.PostLikedOrDislikedAsync(postId, reactionDto, false);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);
                await _reactionHubService.PostLikedOrDislikedAnonymousAsync(postId, null, reaction.ReactionTypeId, UserId);

                return ApiResponses.Success();

            }

        }
        public async Task<ApiResponses<int?>> GetUserReactionTypeIdAsync(int postId, Guid? UserId)
        {
            var reactionTypeId = await _context.PostReaction
                .AsNoTracking()
                .Where( pr=>pr.ReactingUserId == UserId && pr.PostId == postId)
                .Select( pr=>  pr.ReactionTypeId)
                .FirstOrDefaultAsync();

            return ApiResponses<int?>.Success(reactionTypeId);
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
        private async Task<User?> GetReactingUser(Guid UserId)
        {
           return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == UserId);

        }
        private ReactionViewModel ReactionToReactionModel(PostReaction reaction)
        {
            return new ReactionViewModel
            {
                PostId = reaction.PostId,
                ReactingUserId = reaction.ReactingUserId,
                ReactionTypeId = reaction.ReactionTypeId,
                ReactingUserUserName = reaction.ReactingUser.UserName,
                ReactingUserProfileImageUrl = reaction.ReactingUser.ProfilePictureUrl,
                ReactingUserAge = DateTime.Now.Year - reaction.ReactingUser.BirthDate.Year,
                CreatedDate = reaction.CreatedDate,

            };
        }
    }
}
