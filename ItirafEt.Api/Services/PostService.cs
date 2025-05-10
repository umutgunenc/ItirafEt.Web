using System.Runtime.InteropServices;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace ItirafEt.Api.Services
{
    public class PostService
    {
        private readonly dbContext _context;
        private readonly CategoryHubService _categoryHubService;
        public PostService(dbContext context,  CategoryHubService categoryHubService)
        {
            _context = context;
            _categoryHubService = categoryHubService;
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
                CategoryId = dto.CategoryId,
            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            await _categoryHubService.CategoryPostCountChangedAsync(post.CategoryId, true);

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
            //TODO category aktif değilse hata versin postu getirmesin
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.Id == postId && p.IsActive && p.Category.isActive)
                .Select(p => new PostDto
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
            if(post == null)
                return ApiResponses<PostDto>.Fail("Gönderi bulunamadı.");
            return ApiResponses<PostDto>.Success(post);

        }


    }
}
