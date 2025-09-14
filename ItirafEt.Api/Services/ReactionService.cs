using System.Net.NetworkInformation;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Exceptions;

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
                    ReactingUserAge = DateTime.UtcNow.Year - pr.ReactingUser.BirthDate.Year,
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
                .Where(p => p.PostId == podtId && p.ReactionTypeId == (int)ReactionTypeEnum.Like)
                .CountAsync();

            return ApiResponses<int>.Success(likeCount);
        }
        public async Task<ApiResponses<int>> GetPostDislikeCountAsync(int podtId)
        {
            var dislikeCount = await _context.PostReaction
                .AsNoTracking()
                .Where(p => p.PostId == podtId && p.ReactionTypeId == (int)ReactionTypeEnum.Dislike)
                .CountAsync();

            return ApiResponses<int>.Success(dislikeCount);
        }
        public async Task<ApiResponses> LikePostAsync(int postId, Guid userId)
        {
            var post = await GetPostAsync(postId);
            if (post == null)
                return ApiResponses.Fail("Gönderi bulunamadı.");

            var reaction = await GetPostReactionAsync(postId, userId);

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

                reaction.CreatedDate = DateTime.UtcNow;
                _context.PostReaction.Update(reaction);
                await _context.SaveChangesAsync();

                var reactionModel = ReactionToPostReactionModel(reaction);

                await _reactionHubService.PostLikedOrDislikedAsync(reactionModel, true);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);
                await _reactionHubService.PostLikedOrDislikedAnonymousAsync(postId, oldReactionTypeId, reaction.ReactionTypeId);

                return ApiResponses.Success();

            }
            else
            {
                reaction = new PostReaction
                {
                    PostId = postId,
                    ReactingUserId = userId,
                    ReactionTypeId = (int)ReactionTypeEnum.Like,
                    CreatedDate = DateTime.UtcNow
                };
                await _context.PostReaction.AddAsync(reaction);
                likeCount++;

                await _context.SaveChangesAsync();
                reaction.ReactingUser = await GetReactingUserAsync(userId);

                var reactionDto = ReactionToPostReactionModel(reaction);

                await _reactionHubService.PostLikedOrDislikedAsync(reactionDto, false);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);
                await _reactionHubService.PostLikedOrDislikedAnonymousAsync(postId, null, reaction.ReactionTypeId);

                return ApiResponses.Success();

            }


        }
        public async Task<ApiResponses> DislikePostAsync(int postId, Guid userId)
        {
            var post = await GetPostAsync(postId);
            if (post == null)
                return ApiResponses.Fail("Gönderi bulunamadı.");

            var reaction = await GetPostReactionAsync(postId, userId);

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

                reaction.CreatedDate = DateTime.UtcNow;

                _context.PostReaction.Update(reaction);
                await _context.SaveChangesAsync();
                var reactionDto = ReactionToPostReactionModel(reaction);
                await _reactionHubService.PostLikedOrDislikedAsync(reactionDto, true);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);
                await _reactionHubService.PostLikedOrDislikedAnonymousAsync(postId, oldReactionTypeId, reaction.ReactionTypeId);

                return ApiResponses.Success();

            }
            else
            {
                reaction = new PostReaction
                {
                    PostId = postId,
                    ReactingUserId = userId,
                    ReactionTypeId = (int)ReactionTypeEnum.Dislike,
                    CreatedDate = DateTime.UtcNow
                };
                _context.PostReaction.Add(reaction);

                await _context.SaveChangesAsync();

                reaction.ReactingUser = await GetReactingUserAsync(userId);
                var reactionDto = ReactionToPostReactionModel(reaction);
                await _reactionHubService.PostLikedOrDislikedAsync(reactionDto, false);
                await _reactionHubService.UpdatePostLikeCountAsync(post.CategoryId, postId, likeCount);
                await _reactionHubService.PostLikedOrDislikedAnonymousAsync(postId, null, reaction.ReactionTypeId);

                return ApiResponses.Success();

            }

        }
        public async Task<ApiResponses<int?>> GetUserReactionTypeIdAsync(int postId, Guid? userId)
        {
            var reactionTypeId = await _context.PostReaction
                .AsNoTracking()
                .Where(pr => pr.ReactingUserId == userId && pr.PostId == postId)
                .Select(pr => pr.ReactionTypeId)
                .FirstOrDefaultAsync();

            return ApiResponses<int?>.Success(reactionTypeId);
        }
        private async Task<Post?> GetPostAsync(int postId)
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(p => p.PostReactions)
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeletedByAdmin && !p.IsDeletedByUser);
        }
        private async Task<PostReaction?> GetPostReactionAsync(int postId, Guid userId)
        {
            return await _context.PostReaction
                .Include(c => c.ReactingUser)
                .FirstOrDefaultAsync(c => c.PostId == postId && c.ReactingUserId == userId);
        }
        private async Task<User?> GetReactingUserAsync(Guid userId)
        {
            return await _context.Users
                 .AsNoTracking()
                 .FirstOrDefaultAsync(c => c.Id == userId);

        }
        private ReactionViewModel ReactionToPostReactionModel(PostReaction reaction)
        {
            return new ReactionViewModel
            {
                PostId = reaction.PostId,
                ReactingUserId = reaction.ReactingUserId,
                ReactionTypeId = reaction.ReactionTypeId,
                ReactingUserUserName = reaction.ReactingUser.UserName,
                ReactingUserProfileImageUrl = reaction.ReactingUser.ProfilePictureUrl,
                ReactingUserAge = DateTime.UtcNow.Year - reaction.ReactingUser.BirthDate.Year,
                CreatedDate = reaction.CreatedDate,

            };
        }
        public async Task<ApiResponses> LikeCommentAsync(int commentId, Guid userId)
        {
            var comment = await GetCommentAsync(commentId);
            if (comment == null)
                return ApiResponses.Fail("Yorum bulunamadı.");

            var reaction = await GetCommentReactionAsync(commentId, userId);

            var likeCount = comment.CommentReactions.Count(c => c.ReactionId == (int)ReactionTypeEnum.Like);
            if (reaction != null)
            {
                var oldReactionTypeId = reaction.ReactionId;
                if (reaction.ReactionId == (int)ReactionTypeEnum.Like)
                {
                    reaction.ReactionId = (int)ReactionTypeEnum.Cancelled;
                    likeCount--;
                }
                else if (reaction.ReactionId == (int)ReactionTypeEnum.Dislike)
                {
                    reaction.ReactionId = (int)ReactionTypeEnum.Like;
                    likeCount++;
                }
                else
                {
                    reaction.ReactionId = (int)ReactionTypeEnum.Like;
                    likeCount++;
                }

                reaction.CreatedDate = DateTime.UtcNow;
                _context.CommentReactions.Update(reaction);
                await _context.SaveChangesAsync();

                var reactionModel = ReactionToCommentReactionModel(reaction, comment.PostId);

                await _reactionHubService.CommentLikedOrDislikedAsync(reactionModel, true);
                await _reactionHubService.CommentLikedOrDislikedAnonymousAsync(comment.PostId, commentId, oldReactionTypeId, reaction.ReactionId);

                return ApiResponses.Success();

            }
            else
            {
                reaction = new CommentReaction
                {
                    CommentId = commentId,
                    ReactingUserId = userId,
                    ReactionId = (int)ReactionTypeEnum.Like,
                    CreatedDate = DateTime.UtcNow,

                };
                await _context.CommentReactions.AddAsync(reaction);
                likeCount++;

                await _context.SaveChangesAsync();
                reaction.ReactingUser = await GetReactingUserAsync(userId);

                var reactionDto = ReactionToCommentReactionModel(reaction, comment.PostId);

                await _reactionHubService.CommentLikedOrDislikedAsync(reactionDto, false);
                await _reactionHubService.CommentLikedOrDislikedAnonymousAsync(comment.PostId, commentId, null, reaction.ReactionId);

                return ApiResponses.Success();
            }
        }

        public async Task<ApiResponses> DislikeCommentAsync(int commentId, Guid userId)
        {
            var comment = await GetCommentAsync(commentId);
            if (comment == null)
                return ApiResponses.Fail("Yorum bulunamadı.");

            var reaction = await GetCommentReactionAsync(commentId, userId);

            var likeCount = comment.CommentReactions.Count(c => c.ReactionId == (int)ReactionTypeEnum.Like);

            if (reaction != null)
            {
                var oldReactionTypeId = reaction.ReactionId;

                if (reaction.ReactionId == (int)ReactionTypeEnum.Dislike)
                    reaction.ReactionId = (int)ReactionTypeEnum.Cancelled;
                else if (reaction.ReactionId == (int)ReactionTypeEnum.Like)
                {
                    reaction.ReactionId = (int)ReactionTypeEnum.Dislike;
                    likeCount--;
                }
                else
                {
                    reaction.ReactionId = (int)ReactionTypeEnum.Dislike;
                }

                reaction.CreatedDate = DateTime.UtcNow;

                _context.CommentReactions.Update(reaction);
                await _context.SaveChangesAsync();
                var reactionDto = ReactionToCommentReactionModel(reaction, comment.PostId);
                await _reactionHubService.CommentLikedOrDislikedAsync(reactionDto, true);
                await _reactionHubService.CommentLikedOrDislikedAnonymousAsync(comment.PostId, commentId, oldReactionTypeId, reaction.ReactionId);

                return ApiResponses.Success();

            }
            else
            {
                reaction = new CommentReaction
                {
                    CommentId = commentId,
                    ReactingUserId = userId,
                    ReactionId = (int)ReactionTypeEnum.Dislike,
                    CreatedDate = DateTime.UtcNow
                };
                _context.CommentReactions.Add(reaction);

                await _context.SaveChangesAsync();

                reaction.ReactingUser = await GetReactingUserAsync(userId);
                var reactionDto = ReactionToCommentReactionModel(reaction, comment.PostId);
                await _reactionHubService.CommentLikedOrDislikedAsync(reactionDto, false);
                await _reactionHubService.CommentLikedOrDislikedAnonymousAsync(comment.PostId, commentId, null, reaction.ReactionId);

                return ApiResponses.Success();

            }

        }
        private async Task<Comment?> GetCommentAsync(int commentId)
        {
            return await _context.Comments
                .AsNoTracking()
                .Include(c => c.CommentReactions)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.IsActive);
        }
        private async Task<CommentReaction?> GetCommentReactionAsync(int postId, Guid userId)
        {
            return await _context.CommentReactions
                .Include(c => c.ReactingUser)
                .FirstOrDefaultAsync(c => c.CommentId == postId && c.ReactingUserId == userId);
        }
        private ReactionViewModel ReactionToCommentReactionModel(CommentReaction reaction, int postId)
        {
            return new ReactionViewModel
            {
                CommentId = reaction.CommentId,
                ReactingUserId = reaction.ReactingUserId,
                ReactionTypeId = reaction.ReactionId,
                ReactingUserUserName = reaction.ReactingUser.UserName,
                ReactingUserProfileImageUrl = reaction.ReactingUser.ProfilePictureUrl,
                ReactingUserAge = DateTime.UtcNow.Year - reaction.ReactingUser.BirthDate.Year,
                CreatedDate = reaction.CreatedDate,
                PostId = postId
            };
        }
    }

}
