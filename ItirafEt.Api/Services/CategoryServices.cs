using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{

    public class CategoryService
    {
        private readonly dbContext _context;
        private readonly IHubContext<CategoryHub> _hubContext;
        public CategoryService(dbContext context, IHubContext<CategoryHub> hubContext, PostViewService postReadService)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<ApiResponses> CreateCategoryAsync(CategoryDto dto)
        {
 
            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryName == dto.CategoryName.ToUpper()))
                return ApiResponses.Fail("Aynı isimde mevcut bir kategori bulunmaktadır.");

            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryOrder == dto.CategoryOrder))
            {
                var sameOrderCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryOrder == dto.CategoryOrder);
                return ApiResponses.Fail($"Aynı sıra numarasına sahip başka bir kategori bulunmaktadır.\nSıra Numarasını Kontrol Ediniz. Aynı Sıra Numarasına Ait Kategori: {sameOrderCategory.CategoryName}");
            }

            var category = new Category
            {
                CategoryName = dto.CategoryName,
                isActive = true,
                CategoryOrder = (int)dto.CategoryOrder,
                CategoryIconUrl = dto.CategoryIconUrl
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var newCategoryDto = new CategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                isActive = category.isActive,
                CategoryOrder = category.CategoryOrder,
                CategoryIconUrl = category.CategoryIconUrl,
                PostCount = _context.Posts.Count(p => p.CategoryId == category.Id && p.IsActive)
            };

            await _hubContext.Clients.All.SendAsync("ActiveCategoryInformationsChanged", newCategoryDto);

            return ApiResponses.Success();

        }
        public async Task<ApiResponses> EditCategoryAsync(CategoryDto dto)
        {
            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryName == dto.CategoryName.ToUpper() && c.Id != dto.Id))
                return ApiResponses.Fail("Aynı isimde mevcut bir kategori bulunmaktadır.");

            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryOrder == dto.CategoryOrder && c.Id != dto.Id))
            {
                var sameOrderCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryOrder == dto.CategoryOrder && c.Id != dto.Id);
                return ApiResponses.Fail($"Aynı sıra numarasına sahip başka bir kategori bulunmaktadır.\nSıra Numarasını Kontrol Ediniz. Aynı Sıra Numarasına Ait Kategori: {sameOrderCategory.CategoryName}");
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.Id);
            if (category == null)
                return ApiResponses.Fail("Kategori bulunamadı.");


            category.CategoryName = dto.CategoryName;
            category.isActive = dto.isActive;
            category.CategoryOrder = (int)dto.CategoryOrder;
            category.CategoryIconUrl = dto.CategoryIconUrl;
            
            var newCategoryDto = new CategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                isActive = category.isActive,
                CategoryOrder = category.CategoryOrder,
                CategoryIconUrl = category.CategoryIconUrl,
                PostCount = _context.Posts.Count(p => p.CategoryId == category.Id && p.IsActive)
            };

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ActiveCategoryInformationsChanged", newCategoryDto);

            return ApiResponses.Success();

        }
        public async Task<ApiResponses<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    isActive = c.isActive,
                    CategoryOrder = c.CategoryOrder,
                    CategoryIconUrl = c.CategoryIconUrl
                })
                .OrderBy(x => x.CategoryOrder)
                .ToListAsync();

            if (categories == null || categories.Count == 0)
                return ApiResponses<List<CategoryDto>>.Fail("Kategori bulunamadı.");
            return ApiResponses<List<CategoryDto>>.Success(categories);
        }
        public async Task<ApiResponses<List<CategoryDto>>> GetAllActiveCategoriesAsycn()
        {

            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.isActive)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    CategoryOrder = c.CategoryOrder,
                    CategoryIconUrl = c.CategoryIconUrl,
                    isActive = c.isActive,
                    PostCount =  _context.Posts.Count(p => p.CategoryId == c.Id && p.IsActive)
                })
                .ToListAsync();

            if (categories == null || categories.Count == 0)
                return ApiResponses<List<CategoryDto>>.Fail("Kategori bulunamadı.");
            return ApiResponses<List<CategoryDto>>.Success(categories);


        }
        public async Task<ApiResponses<List<PostInfoDto>>> GetCategoryPostsOrderByCreatedDateAsync(int categoryId,int pageNo, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderByDescending(p => p.CreatedDate)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostInfoDto
                {
                    PostId = p.Id,
                    PostTitle = p.Title,
                    PostContentReview = new string(p.Content.Take(50).ToArray()).Trim() + "...",
                    PostCreatedDate = p.CreatedDate,
                    PostCreatorUserName = p.User.UserName
                })
                .ToListAsync();

            if (posts == null || posts.Count == 0)
                return ApiResponses<List<PostInfoDto>>.Fail("Kategoriye ait gönderi bulunamadı.");

            return ApiResponses<List<PostInfoDto>>.Success(posts);
        }

    }
}
