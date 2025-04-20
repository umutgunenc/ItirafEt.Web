using System.Runtime.InteropServices;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace ItirafEt.Api.Services
{
    public class PostService
    {
        private readonly dbContext _context;
        private readonly IHubContext<CategoryHub> _categoryHubContext;
        public PostService(dbContext context, IHubContext<CategoryHub> categoryHubContext)
        {
            _context = context;
            _categoryHubContext = categoryHubContext;
        }

        public async Task<ApiResponses> CreatePostAsync(PostDto dto, Guid UserId)
        {
            if (await _context.Posts.AsNoTracking().AnyAsync(c => c.Title == dto.Title))
                return ApiResponses.Fail("Aynı başlık ile mevcut bir gönderi bulunmaktadır.\nLütfen başlığınızı değiştirin.");

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.isActive);
            if (category == null)
                return ApiResponses.Fail("Seçilen kategori bulunamadı.");

            if (dto.Content.Trim().Length < 100)
                return ApiResponses.Fail("İçerik en az 100 karakter olmalıdır.");
            if (dto.Content.Trim().Length > 4096)
                return ApiResponses.Fail("İçerik en fazla 4096 karakter olmalıdır.");
            if (dto.Title.Trim().Length < 10)
                return ApiResponses.Fail("Başlık en az 10 karakter olmalıdır.");
            if (dto.Title.Trim().Length > 256)
                return ApiResponses.Fail("Başlık en fazla 256 karakter olmalıdır.");


            var post = new Post
            {
                CreatedDate = DateTime.Now,
                Content = dto.Content,
                Title = dto.Title.ToUpper(),
                UserId = UserId,
                IsActive = true,
                IpAddress = dto.IpAddress,
                DeviceInfo = dto.DeviceInfo,
                ViewCount = 0,
                CategoryId = dto.CategoryId,
            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            await _categoryHubContext.Clients.All.SendAsync("ActiveCategoryInformationsChanged");

            return ApiResponses.Success();

        }

        public async Task<ApiResponses<int>> GetCreatedPostIdAsync(Guid UserId)
        {
            var createdPostId = await _context.Posts
                .AsNoTracking()
                .Where(c => c.UserId == UserId)
                .OrderByDescending(c => c.CreatedDate) // En son oluşturulan post en üstte gelecek
                .Select(c => c.Id)                      // Sadece ID'yi seçiyoruz
                .FirstOrDefaultAsync();                 // En üstteki değeri getiriyoruz 

            return ApiResponses<int>.Success(createdPostId);
        }

        public async Task<ApiResponses<PostDto>> GetPostByIdAsync(int postId)
        {

            var post = await _context.Posts
                .AsNoTracking()
                .Where(p => p.Id == postId && p.IsActive)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate,
                    UserName = p.User.UserName,
                    UserId = p.UserId,
                    ViewCount = p.ViewCount,
                    CategoryId = p.CategoryId
                })
                .FirstOrDefaultAsync();
            if(post == null)
                return ApiResponses<PostDto>.Fail("Gönderi bulunamadı.");
            return ApiResponses<PostDto>.Success(post);


            //if (post == null)
            //    return null;

            //var postReactions = await _context.PostReaction
            //    .AsNoTracking()
            //    .Include(pr => pr.ReactionType)
            //    .Include(pr => pr.ReactingUser)
            //    .AsNoTracking()
            //    .Where(pr => pr.PostId == postId)
            //    .Select(pr => new ReactionDto
            //    {
            //        Id = pr.Id,
            //        PostId = pr.PostId,
            //        ReactionTypeId = pr.ReactionTypeId,
            //        ReactingUserId = pr.ReactingUserId,
            //        ReactingUserUserName = pr.ReactingUser.UserName,
            //        CreatedDate = pr.CreatedDate,
            //        ReactionTypeName = pr.ReactionType.Name
            //    })
            //    .ToListAsync();

            //var comments = await _context.Comments
            //    .AsNoTracking()
            //    .Include(c => c.User)
            //    .Include(c => c.CommentReactions)
            //        .ThenInclude(cr => cr.Reaction)
            //    .Include(c => c.CommentReactions)
            //        .ThenInclude(cr => cr.ReactingUser)
            //    .Include(c => c.Replies)
            //        .ThenInclude(r => r.CommentReactions)
            //        .ThenInclude(cr => cr.Reaction)
            //    .Include(c => c.Replies)
            //        .ThenInclude(r => r.CommentReactions)
            //        .ThenInclude(cr => cr.ReactingUser)
            //    .Where(c => c.PostId == postId && c.IsActive)
            //    .Select(c => new CommentsDto
            //    {
            //        Id = c.Id,
            //        Content = c.Content,
            //        CreatedDate = c.CreatedDate,
            //        UpdatedDate = c.UpdatedDate,
            //        UserName = c.User.UserName,
            //        LikeCount = c.LikeCount,
            //        DislikeCount = c.DislikeCount,
            //        CommentCount = c.CommentCount,
            //        ReportCount = c.ReportCount,
            //        CommentRections = c.CommentReactions.Select(cr => new ReactionDto
            //        {
            //            Id = cr.Id,
            //            ReactionTypeId = cr.ReactionId,
            //            ReactingUserId = cr.ReactingUserId,
            //            ReactingUserUserName = cr.ReactingUser.UserName,
            //            CreatedDate = cr.CreatedDate,
            //            ReactionTypeName = cr.Reaction.Name,
            //            CommentId = c.Id
            //        }).ToList(),
            //        CommentReplies = c.Replies.Select(r => new CommentsDto
            //        {
            //            Id = r.Id,
            //            ParentCommentId = r.ParentCommentId,
            //            Content = r.Content,
            //            CreatedDate = r.CreatedDate,
            //            UpdatedDate = r.UpdatedDate,
            //            UserName = r.User.UserName,
            //            LikeCount = r.LikeCount,
            //            DislikeCount = r.DislikeCount,
            //            ReportCount = r.ReportCount,
            //            CommentRections = r.CommentReactions.Select(cr => new ReactionDto
            //            {
            //                Id = cr.Id,
            //                ReactionTypeId = cr.ReactionId,
            //                ReactingUserId = cr.ReactingUserId,
            //                ReactingUserUserName = cr.ReactingUser.UserName,
            //                CreatedDate = cr.CreatedDate,
            //                ReactionTypeName = cr.Reaction.Name,
            //                CommentId = r.Id

            //            }).ToList()

            //        }).ToList()
            //    }).ToListAsync();

            //post.PostReactionDtos = postReactions;
            //post.CommentsDtos = comments;


        }


    }
}
