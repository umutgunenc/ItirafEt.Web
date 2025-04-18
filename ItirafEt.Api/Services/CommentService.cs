using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class CommentService
    {
        private readonly dbContext _context;
        private readonly IHubContext<CommentHub> _commentHub;
        public CommentService(dbContext context, IHubContext<CommentHub> commentHub)
        {
            _context = context;
            _commentHub = commentHub;
        }

        public async Task<List<CommentsDto>?> GetPostCommentsAsync(int postId)
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
               .Where(c => c.PostId == postId && c.IsActive && c.ParentCommentId==null)
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
                   CommentReplies = c.Replies.Select(r => new CommentsDto
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

            return comments;

        }

        public async Task<ApiResponse> AddCommentAsync(int postId, Guid UserId, CommentsDto dto)
        {
            if(string.IsNullOrEmpty(dto.Content))
                return ApiResponse.Fail("Lütfen yorum alanını doldurduktan sonra tekrar deneyin.");

            dto.Content = dto.Content.Trim();

            if (string.IsNullOrEmpty(dto.Content))
                return ApiResponse.Fail("Lütfen yorum alanını doldurduktan sonra tekrar deneyin.");

            var comment = new Comment
            {
                Content = dto.Content,
                CreatedDate = DateTime.UtcNow,
                UserId = UserId,
                PostId = postId,
                ParentCommentId = null,
                LikeCount = 0,
                DislikeCount = 0,
                ReportCount = 0,
                IsActive = true,
            };

            _context.Comments.Add(comment);

            _context.SaveChanges();

            await _commentHub
                .Clients
                .Group($"post-{postId}")
                .SendAsync("CommentAdded", comment.Id);

            return ApiResponse.Success();


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
