using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{

    public class CategoryServices
    {
        private readonly Context _context;
        public CategoryServices(Context context)
        {
            _context = context;
        }

        public async Task<ApiResponse> CreateCategoryAsync(CategoryDto dto)
        {
            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryName == dto.CategoryName.ToUpper()))
                return ApiResponse.Fail("Aynı isimde mevcut bir kategori bulunmaktadır.");

            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryOrder == dto.CategoryOrder))
            {
                var sameOrderCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryOrder == dto.CategoryOrder);
                return ApiResponse.Fail($"Aynı sıra numarasına sahip başka bir kategori bulunmaktadır.\nSıra Numarasını Kontrol Ediniz. Aynı Sıra Numarasına Ait Kategori: {sameOrderCategory.CategoryName}");
            }

            var category = new Category
            {
                CategoryName = dto.CategoryName.ToUpper(),
                isActive = true,
                CategoryOrder = (int)dto.CategoryOrder,
                CategoryIconUrl = dto.CategoryIconUrl
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return ApiResponse.Success();

        }
        public async Task<ApiResponse> EditCategoryAsync(CategoryDto dto)
        {
            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryName == dto.CategoryName.ToUpper() && c.Id != dto.Id))
                return ApiResponse.Fail("Aynı isimde mevcut bir kategori bulunmaktadır.");

            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryOrder == dto.CategoryOrder && c.Id != dto.Id))
            {
                var sameOrderCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryOrder == dto.CategoryOrder && c.Id != dto.Id);
                return ApiResponse.Fail($"Aynı sıra numarasına sahip başka bir kategori bulunmaktadır.\nSıra Numarasını Kontrol Ediniz. Aynı Sıra Numarasına Ait Kategori: {sameOrderCategory.CategoryName}");
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.Id);
            if (category == null)
                return ApiResponse.Fail("Kategori bulunamadı.");


            category.CategoryName = dto.CategoryName;
            category.isActive = dto.isActive;
            category.CategoryOrder = (int)dto.CategoryOrder;
            category.CategoryIconUrl = dto.CategoryIconUrl;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return ApiResponse.Success();

        }
        public async Task<List<CategoryDto>> GetCategoryAsync()
        {
            return await _context.Categories.AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    isActive = c.isActive,
                    CategoryOrder = c.CategoryOrder,
                    CategoryIconUrl = c.CategoryIconUrl
                }).ToListAsync();
        }
        public async Task<List<CategoryDto>> GetAllActiveCategoryAsycn()
        {
            return await _context.Categories.AsNoTracking()
                .Where(c => c.isActive)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    CategoryOrder = c.CategoryOrder,
                    CategoryIconUrl = c.CategoryIconUrl
                })
                .OrderBy(x => x.CategoryOrder)
                .ToListAsync();
        }
    }
}
