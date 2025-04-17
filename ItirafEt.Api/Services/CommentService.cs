using ItirafEt.Api.Data;
using ItirafEt.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class CommentService
    {
        private readonly dbContext _context;
        public CommentService(dbContext context)
        {
            _context = context;
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
               .Where(c => c.PostId == postId && c.IsActive)
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

    }
}
