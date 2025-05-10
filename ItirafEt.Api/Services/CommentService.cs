using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class CommentService
    {
        private readonly dbContext _context;
        private readonly CommentHubService _commentHubServices;
        public CommentService(dbContext context, CommentHubService commentHubServices)
        {
            _context = context;
            _commentHubServices = commentHubServices;
        }

        public async Task<ApiResponses<List<CommentsDto>>> GetPostCommentsAsync(int postId)
        {

            var comments = await _context.Comments
               .AsNoTracking()
               .Include(c => c.User)
               .Include(c => c.CommentReactions)
                   .ThenInclude(cr => cr.Reaction)
               .Include(c => c.CommentReactions)
                   .ThenInclude(cr => cr.ReactingUser)
               .Include(c => c.Replies)
                   .ThenInclude(r => r.CommentReactions)
                   .ThenInclude(cr => cr.Reaction)
               .Include(c => c.Replies)
                   .ThenInclude(r => r.CommentReactions)
                   .ThenInclude(cr => cr.ReactingUser)
               .Where(c => c.PostId == postId && c.IsActive && c.ParentCommentId == null)
               .Select(c => new CommentsDto
               {
                   Id = c.Id,
                   Content = c.Content,
                   CreatedDate = c.CreatedDate,
                   UpdatedDate = c.UpdatedDate,
                   UserName = c.User.UserName,
                   LikeCount = c.LikeCount,
                   DislikeCount = c.DislikeCount,
                   CommentCount = c.CommentCount,
                   ReportCount = c.ReportCount,
                   CommentRections = c.CommentReactions.Select(cr => new ReactionDto
                   {
                       Id = cr.Id,
                       ReactionTypeId = cr.ReactionId,
                       ReactingUserId = cr.ReactingUserId,
                       ReactingUserUserName = cr.ReactingUser.UserName,
                       CreatedDate = cr.CreatedDate,
                       ReactionTypeName = cr.Reaction.Name,
                       CommentId = c.Id
                   }).ToList(),
                   CommentReplies = c.Replies
                        .Where(r => r.IsActive)
                        .Select(r => new CommentsDto
                        {
                            Id = r.Id,
                            ParentCommentId = r.ParentCommentId,
                            Content = r.Content,
                            CreatedDate = r.CreatedDate,
                            UpdatedDate = r.UpdatedDate,
                            UserName = r.User.UserName,
                            LikeCount = r.LikeCount,
                            DislikeCount = r.DislikeCount,
                            ReportCount = r.ReportCount,
                            CommentRections = r.CommentReactions.Select(cr => new ReactionDto
                            {
                                Id = cr.Id,
                                ReactionTypeId = cr.ReactionId,
                                ReactingUserId = cr.ReactingUserId,
                                ReactingUserUserName = cr.ReactingUser.UserName,
                                CreatedDate = cr.CreatedDate,
                                ReactionTypeName = cr.Reaction.Name,
                                CommentId = r.Id

                            }).ToList()

                        }).ToList()
               }).ToListAsync();


            if (comments == null || comments.Count == 0)
                return ApiResponses<List<CommentsDto>>.Fail("Henüz Yorum Yok. İlk yorumu siz yapın!");
            return ApiResponses<List<CommentsDto>>.Success(comments);
        }

        public async Task<ApiResponses> AddCommentAsync(int postId, Guid UserId, CommentsDto dto)
        {
            if (string.IsNullOrEmpty(dto.Content))
                return ApiResponses.Fail("Lütfen yorum alanını doldurduktan sonra tekrar deneyin.");

            dto.Content = dto.Content.Trim();

            if (string.IsNullOrEmpty(dto.Content))
                return ApiResponses.Fail("Lütfen yorum alanını doldurduktan sonra tekrar deneyin.");

            var comment = new Comment
            {
                Content = dto.Content,
                CreatedDate = DateTime.Now,
                UserId = UserId,
                PostId = postId,
                ParentCommentId = null,
                LikeCount = 0,
                DislikeCount = 0,
                ReportCount = 0,
                IsActive = true,
                DeviceInfo = dto.DeviceInfo,
                IpAddress = dto.IpAddress
            };

            _context.Comments.Add(comment);

            await _context.SaveChangesAsync();

            dto.CreatedDate = comment.CreatedDate;

            dto.UserName = await _context.Users
                .AsNoTracking()
                .Include(u=>u.Comments)
                .Where(u => u.Id == UserId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            await _commentHubServices.CommentAddedOrDeletedAsync(postId,dto,true);

            return ApiResponses.Success();


            //var currentPostCommentCount = await _context.Posts
            //    .Where(p => p.Id == postId)
            //    .Select(p => p.CommentCount)
            //    .FirstOrDefaultAsync();

            //currentPostCommentCount++;

            //var post = await _context.Posts.FindAsync(postId);
            //if (post != null)
            //{
            //    post.CommentCount = currentPostCommentCount;
            //    _context.Posts.Update(post);
            //}


        }

    }
}
