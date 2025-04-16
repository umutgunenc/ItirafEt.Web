using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared.DTOs;
using Microsoft.EntityFrameworkCore;


namespace ItirafEt.Api.Services
{
    public class PostService
    {
        private readonly dbContext _context;
        public PostService(dbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> CreatePostAsync(PostDto dto, Guid UserId)
        {
            if (await _context.Posts.AsNoTracking().AnyAsync(c => c.Title == dto.Title))
                return ApiResponse.Fail("Aynı başlık ile mevcut bir gönderi bulunmaktadır.\nLütfen başlığınızı değiştirin.");

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.isActive);
            if (category == null)
                return ApiResponse.Fail("Seçilen kategori bulunamadı.");

            if (dto.Content.Trim().Length < 100)
                return ApiResponse.Fail("İçerik en az 100 karakter olmalıdır.");
            if (dto.Content.Trim().Length > 4096)
                return ApiResponse.Fail("İçerik en fazla 4096 karakter olmalıdır.");
            if (dto.Title.Trim().Length < 10)
                return ApiResponse.Fail("Başlık en az 10 karakter olmalıdır.");
            if (dto.Title.Trim().Length > 256)
                return ApiResponse.Fail("Başlık en fazla 256 karakter olmalıdır.");


            var post = new Post
            {
                CommentCount = 0,
                CreatedDate = DateTime.Now,
                Content = dto.Content,
                Title = dto.Title.ToUpper(),
                UserId = UserId,
                IsActive = true,
                IpAddress = dto.IpAddress,
                DeviceInfo = dto.DeviceInfo,
                ViewCount = 0,
                LikeCount = 0,
                DislikeCount = 0,
                ReportCount = 0,
                CategoryId = dto.CategoryId,
            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return ApiResponse.Success();

        }

        public async Task<ApiResponse> GetCreatedPostAsync(Guid UserId)
        {
            var createdPost = await _context.Posts
                .AsNoTracking()
                .Where(c => c.UserId == UserId)
                .OrderByDescending(c => c.CreatedDate) // En son oluşturulan post en üstte gelecek
                .Select(c => c.Id)                      // Sadece ID'yi seçiyoruz
                .FirstOrDefaultAsync();                 // En üstteki değeri getiriyoruz 

            return ApiResponse.Success(createdPost.ToString());
        }
    }
}
