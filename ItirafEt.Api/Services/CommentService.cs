using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.ViewModels;
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

        public async Task<ApiResponses<List<CommentsViewModel>>> GetPostCommentsAsync(int postId)
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
               .Where(c => c.PostId == postId && c.ParentCommentId == null)
               .Select(c => new CommentsViewModel
               {
                   Id = c.Id,
                   Content = c.Content,
                   CreatedDate = c.CreatedDate,
                   UpdatedDate = c.UpdatedDate,
                   UserName = c.User.UserName,
                   UserId = c.UserId,
                   CommentUserProfilPhotoUrl = c.User.ProfilePictureUrl,
                   ShowReplies = false,
                   IsActive = c.IsActive,
                   CommentRections = c.CommentReactions.Select(cr => new ReactionViewModel
                   {
                       Id = cr.Id,
                       ReactionTypeId = cr.ReactionId,
                       ReactingUserId = cr.ReactingUserId,
                       ReactingUserUserName = cr.ReactingUser.UserName,
                       CreatedDate = cr.CreatedDate,
                       CommentId = c.Id,


                   }).ToList(),
                   CommentReplies = c.Replies
                        .Select(r => new CommentsViewModel
                        {
                            Id = r.Id,
                            ParentCommentId = r.ParentCommentId,
                            Content = r.Content,
                            CreatedDate = r.CreatedDate,
                            UpdatedDate = r.UpdatedDate,
                            UserName = r.User.UserName,
                            UserId = r.UserId,
                            CommentUserProfilPhotoUrl = r.User.ProfilePictureUrl,
                            IsActive = r.IsActive,
                            CommentRections = r.CommentReactions.Select(cr => new ReactionViewModel
                            {
                                Id = cr.Id,
                                ReactionTypeId = cr.ReactionId,
                                ReactingUserId = cr.ReactingUserId,
                                ReactingUserUserName = cr.ReactingUser.UserName,
                                CreatedDate = cr.CreatedDate,
                                CommentId = r.Id

                            }).ToList()

                        }).ToList()
               }).ToListAsync();


            if (comments == null || comments.Count == 0)
                return ApiResponses<List<CommentsViewModel>>.Fail("Henüz Yorum Yok. İlk yorumu siz yapın!");
            foreach (var comment in comments)
            {
                if (comment.CommentReplies != null && comment.CommentReplies.Count > 0)
                    comment.AnyReplies = true;
                else
                    comment.AnyReplies = false;
            }
            return ApiResponses<List<CommentsViewModel>>.Success(comments);
        }

        public async Task<ApiResponses> AddCommentAsync(int postId, Guid userId, CommentsViewModel model)
        {
            if (string.IsNullOrEmpty(model.Content))
                return ApiResponses.Fail("Lütfen yorum alanını doldurduktan sonra tekrar deneyin.");

            model.Content = model.Content.Trim();

            if (string.IsNullOrEmpty(model.Content))
                return ApiResponses.Fail("Lütfen yorum alanını doldurduktan sonra tekrar deneyin.");

            var comment = new Comment
            {
                Content = model.Content,
                CreatedDate = DateTime.UtcNow,
                UserId = userId,
                PostId = postId,
                ParentCommentId = null,
                IsActive = true,
                DeviceInfo = model.DeviceInfo,
                IpAddress = model.IpAddress,
            };

            _context.Comments.Add(comment);

            await _context.SaveChangesAsync();

            model.CreatedDate = comment.CreatedDate;
            model.Id = comment.Id;
            model.AnyReplies = false;
            model.ShowReplies = false;
            model.IsActive = true;
            model.UserName = await GetUserNameAsync(userId);
            model.UserId = userId;
            model.CommentUserProfilPhotoUrl = await GetUserProfilePhotoUrl(userId);

            await _commentHubServices.CommentAddedOrEditedAsync(postId, model, true);

            return ApiResponses.Success();

        }

        public async Task<ApiResponses> AddCommentReplyAsync(int postId, int commentId, Guid userId, CommentsViewModel replyDto)
        {
            if (string.IsNullOrEmpty(replyDto.Content))
                return ApiResponses.Fail("Lütfen yorum alanını doldurduktan sonra tekrar deneyin.");

            replyDto.Content = replyDto.Content.Trim();

            if (string.IsNullOrEmpty(replyDto.Content))
                return ApiResponses.Fail("Lütfen yorum alanını doldurduktan sonra tekrar deneyin.");

            var didHaveCommentComment = await _context.Comments
                .AsNoTracking()
                .AnyAsync(c => c.Id == commentId && c.IsActive);

            if (!didHaveCommentComment)
                return ApiResponses.Fail("Cevaplamak istediğiniz yorum silindiği için, bu yoruma cevap yazamazsınız.");

            var reply = new Comment
            {
                Content = replyDto.Content,
                CreatedDate = DateTime.UtcNow,
                UserId = userId,
                PostId = postId,
                ParentCommentId = commentId,
                IsActive = true,
                DeviceInfo = replyDto.DeviceInfo,
                IpAddress = replyDto.IpAddress
            };

            _context.Comments.Add(reply);

            await _context.SaveChangesAsync();

            replyDto.CreatedDate = reply.CreatedDate;
            replyDto.UserName = await GetUserNameAsync(userId);
            replyDto.ParentCommentId = commentId;
            replyDto.Id = reply.Id;
            replyDto.CommentUserProfilPhotoUrl = await GetUserProfilePhotoUrl(userId);
            replyDto.UserId = userId;
            replyDto.IsActive = true;
            await _commentHubServices.ReplyAddedOrEditedAsync(postId, replyDto, true);

            return ApiResponses.Success();

        }

        private async Task<string?> GetUserProfilePhotoUrl(Guid userId)
        {
            return await _context.Users
                  .AsNoTracking()
                  .Where(u => u.Id == userId)
                  .Select(u => u.ProfilePictureUrl)
                  .FirstOrDefaultAsync();
        }

        private async Task<string?> GetUserNameAsync(Guid userId)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();
        }


        public async Task<ApiResponses> DeleteCommentAsync(CommentsViewModel commentModel, Guid userId)
        {
            var comment = await _context.Comments
                .Where(c => c.Id == commentModel.Id && c.IsActive)
                .FirstOrDefaultAsync();
            if (comment == null)
                return ApiResponses.Fail("Yorum bulunamadı veya silinmiş.");


            CommentHistory commentHistory = new();
            commentHistory.CommentId = comment.Id;
            commentHistory.DeviceInfo = comment.DeviceInfo;
            commentHistory.IpAddress = comment.IpAddress;
            commentHistory.OperationDate = DateTime.UtcNow;
            commentHistory.OperationByUserId = userId;
            commentHistory.Content = comment.Content;

            _context.CommentHistories.Add(commentHistory);


            if (userId == comment.UserId)
                comment.Content = "Bu yorum, sahibi tarafından silinmiştir.";
            else
                comment.Content = "Bu yorum, site yönetimi tarafından silinmiştir.";

            comment.IsActive = false;
            comment.UpdatedDate = commentHistory.OperationDate;
            await _context.SaveChangesAsync();

            commentModel.UpdatedDate = commentHistory.OperationDate;
            commentModel.IsActive = false;
            commentModel.Content = comment.Content;
            commentModel.ShowDeleteWarning = false;
            commentModel.IsCommentEditing = false;

            if (comment.ParentCommentId == null)
                await _commentHubServices.CommentAddedOrEditedAsync(comment.PostId, commentModel, false);
            else
            {
                commentModel.ParentCommentId = commentModel.ParentCommentId;
                await _commentHubServices.ReplyAddedOrEditedAsync(comment.PostId, commentModel, false);
            }

            return ApiResponses.Success();

        }

        public async Task<ApiResponses> EditCommentAsync(CommentsViewModel commentModel, Guid userId)
        {
            var comment = await _context.Comments
                .Where(c => c.Id == commentModel.Id && c.IsActive && c.UserId == userId)
                .FirstOrDefaultAsync();

            if (comment == null)
                return ApiResponses.Fail("Yorum bulunamadı veya silinmiş.");

            commentModel.Content = commentModel.Content.Trim();

            if (string.IsNullOrEmpty(commentModel.Content))
                return ApiResponses.Fail("Lütfen yorum alanını doldurduktan sonra tekrar deneyin.");

            CommentHistory commentHistory = new();
            commentHistory.CommentId = comment.Id;
            commentHistory.DeviceInfo = comment.DeviceInfo;
            commentHistory.IpAddress = comment.IpAddress;
            commentHistory.OperationDate = DateTime.UtcNow;
            commentHistory.OperationByUserId = userId;
            commentHistory.Content = comment.Content;

            await _context.CommentHistories.AddAsync(commentHistory);

            comment.Content = commentModel.Content;
            comment.UpdatedDate = DateTime.UtcNow;

            _context.Comments.Update(comment);

            await _context.SaveChangesAsync();

            commentModel.UpdatedDate = comment.UpdatedDate;
            commentModel.ShowDeleteWarning = false;
            commentModel.IsCommentEditing = false;

            if (comment.ParentCommentId == null)
                await _commentHubServices.CommentAddedOrEditedAsync(comment.PostId, commentModel, false);
            else
            {
                commentModel.ParentCommentId = comment.ParentCommentId;
                await _commentHubServices.ReplyAddedOrEditedAsync(comment.PostId, commentModel, false);
            }
            return ApiResponses.Success();

        }
    }
}
