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
                CreatedDate = DateTime.Now,
                Content = model.Content,
                Title = model.Title.ToUpper(),
                UserId = userId,
                IsActive = true,
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
                .Where(p => p.Id == postId && p.IsActive && p.Category.isActive)
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
                    UserAge = DateTime.Now.Year - p.User.BirthDate.Year,
                    UserGenderId = p.User.Gender.Id,
                    UserProfileImageUrl = p.User.ProfilePictureUrl
                })
                .FirstOrDefaultAsync();
            if (post == null)
                return ApiResponses<PostViewModel>.Fail("Gönderi bulunamadı.");
            return ApiResponses<PostViewModel>.Success(post);

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
                    PostContentReview = p.Content.Length > 100 ? p.Content.Substring(0, 100) + "..." : p.Content ,
                    PostId = p.Id,
                    PostTitle = p.Title.Length > 50 ? p.Title.Substring(0, 30) + "..." : p.Title,
                    PostCreatedDate = p.CreatedDate,
                    PostLikeCount = _context.PostReaction.Count(pr => pr.PostId == p.Id && pr.ReactionTypeId == (int)ReactionTypeEnum.Like),
                    PostViewCount = _context.UserReadPosts.Count(ur => ur.PostId == p.Id),
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


    }
}
