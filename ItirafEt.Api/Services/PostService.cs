using System.Runtime.InteropServices;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace ItirafEt.Api.Services
{
    public class PostService
    {
        private readonly dbContext _context;
        private readonly CategoryHubService _categoryHubService;
        public PostService(dbContext context, CategoryHubService categoryHubService)
        {
            _context = context;
            _categoryHubService = categoryHubService;
        }

        public async Task<ApiResponses> CreatePostAsync(PostViewModel model, Guid userId)
        {
            if (await _context.Posts.AsNoTracking().AnyAsync(c => c.Title == model.Title))
                return ApiResponses.Fail("Aynı başlık ile mevcut bir gönderi bulunmaktadır.\nLütfen başlığınızı değiştirin.");

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId && c.isActive);
            if (category == null)
                return ApiResponses.Fail("Seçilen kategori bulunamadı.");

            if (model.Content.Trim().Length < 100)
                return ApiResponses.Fail("İçerik en az 100 karakter olmalıdır.");
            if (model.Content.Trim().Length > 4096)
                return ApiResponses.Fail("İçerik en fazla 4096 karakter olmalıdır.");
            if (model.Title.Trim().Length < 10)
                return ApiResponses.Fail("Başlık en az 10 karakter olmalıdır.");
            if (model.Title.Trim().Length > 256)
                return ApiResponses.Fail("Başlık en fazla 256 karakter olmalıdır.");


            var post = new Post
            {
                CreatedDate = DateTime.UtcNow,
                Content = model.Content,
                Title = model.Title.ToUpper(),
                UserId = userId,
                IsDeletedByUser = false,
                IsDeletedByAdmin = false,
                IpAddress = model.IpAddress,
                DeviceInfo = model.DeviceInfo,
                CategoryId = model.CategoryId,
            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            await _categoryHubService.CategoryPostCountChangedAsync(post.CategoryId, true);

            return ApiResponses.Success();

        }

        public async Task<ApiResponses<int>> GetCreatedPostIdAsync(Guid userId)
        {
            var createdPostId = await _context.Posts
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedDate) // En son oluşturulan post en üstte gelecek
                .Select(c => c.Id)                      // Sadece ID'yi seçiyoruz
                .FirstOrDefaultAsync();                 // En üstteki değeri getiriyoruz 

            return ApiResponses<int>.Success(createdPostId);
        }

        public async Task<ApiResponses<PostViewModel>> GetPostByIdAsync(int postId)
        {
            //TODO category aktif değilse hata versin postu getirmesin
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.Id == postId && !p.IsDeletedByUser && !p.IsDeletedByAdmin && p.Category.isActive)
                .Select(p => new PostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate,
                    UserName = p.User.UserName,
                    UserId = p.UserId,
                    CategoryId = p.CategoryId,
                    UserAge = DateTime.UtcNow.Year - p.User.BirthDate.Year,
                    UserGenderId = p.User.Gender.Id,
                    UserProfileImageUrl = p.User.ProfilePictureUrl
                })
                .FirstOrDefaultAsync();
            if (post == null)
                return ApiResponses<PostViewModel>.Fail("Gönderi bulunamadı.");
            return ApiResponses<PostViewModel>.Success(post);

        }

        public async Task<ApiResponses<EditPostViewModel>> GetPostInformationByIdAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.Id == postId && !p.IsDeletedByUser && !p.IsDeletedByAdmin && p.Category.isActive)
                .Select(p => new EditPostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    UpdatedDate = p.UpdatedDate,
                    CategoryId = p.CategoryId,
                })
                .FirstOrDefaultAsync();

            if (post == null)
                return ApiResponses<EditPostViewModel>.Fail("Gönderi bulunamadı.");
            return ApiResponses<EditPostViewModel>.Success(post);

        }

        public async Task<ApiResponses<UserPostsViewModel>> GetUserPostsAsync(Guid userId, int size, int page)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && !u.IsBanned);

            if (user == null)
                return ApiResponses<UserPostsViewModel>.Fail("Kullanıcı Bulunamadı.");

            var totalPosts = await _context.Posts
                .AsNoTracking()
                .CountAsync(p => p.UserId == userId);

            if (totalPosts == 0)
                return ApiResponses<UserPostsViewModel>.Fail("Henüz Bir Gönderi Paylaşmadınız.");

            var posts = await _context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedDate)
                .Skip(size * (page - 1))
                .Take(size)
                .Select(p => new ListOfUserPost
                {
                    PostContentReview = p.Content.Length > 100 ? p.Content.Substring(0, 100) + "..." : p.Content,
                    PostId = p.Id,
                    PostTitle = p.Title.Length > 30 ? p.Title.Substring(0, 30) + "..." : p.Title,
                    PostCreatedDate = p.CreatedDate,
                    PostLikeCount = _context.PostReaction.Count(pr => pr.PostId == p.Id && pr.ReactionTypeId == (int)ReactionTypeEnum.Like),
                    PostViewCount = _context.UserReadPosts.Count(ur => ur.PostId == p.Id),
                    IsDeletedByAdmin = p.IsDeletedByAdmin,
                    IsDeletedByUser = p.IsDeletedByUser,
                })
                .ToListAsync();

            var hasMore = totalPosts > size * page;

            var userPostsViewModel = new UserPostsViewModel
            {
                HasNextPage = hasMore,
                TotalCount = totalPosts,
                UserName = user.UserName,
                UserProfilePicture = user.ProfilePictureUrl,
                UserPosts = posts
            };

            return ApiResponses<UserPostsViewModel>.Success(userPostsViewModel);
        }

        public async Task<ApiResponses> HidePostAsync(int postId, Guid userId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
            if (post == null)
                return ApiResponses.Fail("Gönderi bulunamadı.");
            post.IsDeletedByUser = true;
            await _context.SaveChangesAsync();
            await _categoryHubService.CategoryPostCountChangedAsync(post.CategoryId, false);
            return ApiResponses.Success();
        }

        public async Task<ApiResponses> ShowPostAsync(int postId, Guid userId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
            if (post == null)
                return ApiResponses.Fail("Gönderi bulunamadı.");
            post.IsDeletedByUser = false;
            await _context.SaveChangesAsync();
            await _categoryHubService.CategoryPostCountChangedAsync(post.CategoryId, true);
            return ApiResponses.Success();
        }

        public async Task<ApiResponses> CanUserEditPostAsync(int postId, Guid userId)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId && !p.IsDeletedByUser && !p.IsDeletedByAdmin);
            if (post == null)
                return ApiResponses.Fail("Gönderi bulunamadı.");
            return ApiResponses.Success();
        }

        public async Task<ApiResponses> EditPostAsync(EditPostViewModel model)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == model.Id && !p.IsDeletedByUser && !p.IsDeletedByAdmin);

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId && c.isActive);
            if (category == null)
                return ApiResponses.Fail("Seçilen kategori bulunamadı.");
            if (post == null)
                return ApiResponses.Fail("Gönderi bulunamadı.");
            if (model.Content.Trim().Length < 100)
                return ApiResponses.Fail("İçerik en az 100 karakter olmalıdır.");
            if (model.Content.Trim().Length > 4096)
                return ApiResponses.Fail("İçerik en fazla 4096 karakter olmalıdır.");
            if (model.Title.Trim().Length < 10)
                return ApiResponses.Fail("Başlık en az 10 karakter olmalıdır.");
            if (model.Title.Trim().Length > 256)
                return ApiResponses.Fail("Başlık en fazla 256 karakter olmalıdır.");
            post.Title = model.Title.ToUpper();
            post.Content = model.Content;
            post.UpdatedDate = DateTime.UtcNow;
            post.CategoryId = model.CategoryId;
            post.IpAddress = model.IpAddress;
            post.DeviceInfo = model.DeviceInfo;
            await _context.SaveChangesAsync();
            return ApiResponses.Success();
        }
    }
}
