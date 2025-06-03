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

    public class CategoryService
    {
        private readonly dbContext _context;
        private readonly CategoryHubService _categoryHubService;
        public CategoryService(dbContext context, IHubContext<CategoryHub> hubContext, PostViewService postReadService, CategoryHubService categoryHubService)
        {
            _context = context;
            _categoryHubService = categoryHubService;
        }

        public async Task<ApiResponses> CreateCategoryAsync(CategoryViewModel model)
        {

            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryName == model.CategoryName.ToUpper()))
                return ApiResponses.Fail("Aynı isimde mevcut bir kategori bulunmaktadır.");

            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryOrder == model.CategoryOrder))
            {
                var sameOrderCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryOrder == model.CategoryOrder);
                return ApiResponses.Fail($"Aynı sıra numarasına sahip başka bir kategori bulunmaktadır.\nSıra Numarasını Kontrol Ediniz. Aynı Sıra Numarasına Ait Kategori: {sameOrderCategory.CategoryName}");
            }

            var category = new Category
            {
                CategoryName = model.CategoryName,
                isActive = true,
                CategoryOrder = (int)model.CategoryOrder,
                CategoryIconUrl = model.CategoryIconUrl
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var newCategoryModel = new CategoryViewModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                isActive = category.isActive,
                CategoryOrder = category.CategoryOrder,
                CategoryIconUrl = category.CategoryIconUrl,
                PostCount = _context.Posts.Count(p => p.CategoryId == category.Id && p.IsActive)
            };

            //await _hubContext.Clients.All.SendAsync("ActiveCategoryInformationsChanged", newCategoryDto);
            await _categoryHubService.CategoryInfoChangedAsync(newCategoryModel);

            return ApiResponses.Success();

        }
        public async Task<ApiResponses> EditCategoryAsync(CategoryViewModel model)
        {
            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryName == model.CategoryName.ToUpper() && c.Id != model.Id))
                return ApiResponses.Fail("Aynı isimde mevcut bir kategori bulunmaktadır.");

            if (await _context.Categories.AsNoTracking().AnyAsync(c => c.CategoryOrder == model.CategoryOrder && c.Id != model.Id))
            {
                var sameOrderCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryOrder == model.CategoryOrder && c.Id != model.Id);
                return ApiResponses.Fail($"Aynı sıra numarasına sahip başka bir kategori bulunmaktadır.\nSıra Numarasını Kontrol Ediniz. Aynı Sıra Numarasına Ait Kategori: {sameOrderCategory.CategoryName}");
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == model.Id);
            if (category == null)
                return ApiResponses.Fail("Kategori bulunamadı.");


            category.CategoryName = model.CategoryName;
            category.isActive = model.isActive;
            category.CategoryOrder = (int)model.CategoryOrder;
            category.CategoryIconUrl = model.CategoryIconUrl;

            var newCategoryDto = new CategoryViewModel
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
            await _categoryHubService.CategoryInfoChangedAsync(newCategoryDto);
            return ApiResponses.Success();

        }
        public async Task<ApiResponses<List<CategoryViewModel>>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.AsNoTracking()
                .Select(c => new CategoryViewModel
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
                return ApiResponses<List<CategoryViewModel>>.Fail("Kategori bulunamadı.");
            return ApiResponses<List<CategoryViewModel>>.Success(categories);
        }
        public async Task<ApiResponses<List<CategoryViewModel>>> GetAllActiveCategoriesAsycn()
        {

            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.isActive)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    CategoryOrder = c.CategoryOrder,
                    CategoryIconUrl = c.CategoryIconUrl,
                    isActive = c.isActive,
                    PostCount = _context.Posts.Count(p => p.CategoryId == c.Id && p.IsActive)
                })
                .ToListAsync();

            if (categories == null || categories.Count == 0)
                return ApiResponses<List<CategoryViewModel>>.Fail("Kategori bulunamadı.");
            return ApiResponses<List<CategoryViewModel>>.Success(categories);


        }
        public async Task<ApiResponses<List<PostInfoViewModel>>> GetCategoryPostsOrderByCreatedDateAsync(int categoryId, int pageNo, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.IsActive && p.Category.isActive)
                .OrderByDescending(p => p.CreatedDate)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostInfoViewModel
                {
                    PostId = p.Id,
                    PostTitle = p.Title,
                    PostContentReview = new string(p.Content.Take(100).ToArray()).Trim() + "...",
                    PostCreatedDate = p.CreatedDate,
                    PostCreatorUserName = p.User.UserName,
                    PostCreatorProfilPicture = p.User.ProfilePictureUrl,
                    PostViewCount = p.Readers.Count,
                    PostLikeCount = p.PostReactions
                        .Where(pr => pr.ReactionTypeId == (int)ReactionTypeEnum.Like)
                        .Count()
                })
                .ToListAsync();

            if (posts == null || posts.Count == 0)
                return ApiResponses<List<PostInfoViewModel>>.Fail("Kategoriye ait gönderi bulunamadı.");

            return ApiResponses<List<PostInfoViewModel>>.Success(posts);
        }
        public async Task<ApiResponses<List<PostInfoViewModel>>> GetCategoryPostsOrderByViewCountAsync(int categoryId, int pageNo, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.IsActive && p.Category.isActive)
                .OrderByDescending(p => p.Readers.Count)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostInfoViewModel
                {
                    PostId = p.Id,
                    PostTitle = p.Title,
                    PostContentReview = new string(p.Content.Take(100).ToArray()).Trim() + "...",
                    PostCreatedDate = p.CreatedDate,
                    PostCreatorUserName = p.User.UserName,
                    PostCreatorProfilPicture = p.User.ProfilePictureUrl,
                    PostViewCount = p.Readers.Count,
                    PostLikeCount = p.PostReactions
                        .Where(pr => pr.ReactionTypeId == (int)ReactionTypeEnum.Like)
                        .Count()
                })
                .ToListAsync();

            if (posts == null || posts.Count == 0)
                return ApiResponses<List<PostInfoViewModel>>.Fail("Kategoriye ait gönderi bulunamadı.");

            return ApiResponses<List<PostInfoViewModel>>.Success(posts);
        }
        public async Task<ApiResponses<List<PostInfoViewModel>>> GetCategoryPostsOrderByLikeCountAsync(int categoryId, int pageNo, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.IsActive && p.Category.isActive)
                .OrderByDescending(p => p.PostReactions
                        .Where(pr => pr.ReactionTypeId == (int)ReactionTypeEnum.Like)
                        .Count())
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostInfoViewModel
                {
                    PostId = p.Id,
                    PostTitle = p.Title,
                    PostContentReview = new string(p.Content.Take(100).ToArray()).Trim() + "...",
                    PostCreatedDate = p.CreatedDate,
                    PostCreatorUserName = p.User.UserName,
                    PostCreatorProfilPicture = p.User.ProfilePictureUrl,
                    PostViewCount = p.Readers.Count,
                    PostLikeCount = p.PostReactions
                        .Where(pr => pr.ReactionTypeId == (int)ReactionTypeEnum.Like)
                        .Count()
                })
                .ToListAsync();

            if (posts == null || posts.Count == 0)
                return ApiResponses<List<PostInfoViewModel>>.Fail("Kategoriye ait gönderi bulunamadı.");

            return ApiResponses<List<PostInfoViewModel>>.Success(posts);
        }
        public async Task<ApiResponses<string>> GetCategoryNameAsync(int categoryId)
        {
            var categoryName = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == categoryId && c.isActive)
                .Select(c => c.CategoryName)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(categoryName))
                return ApiResponses<string>.Fail("Kategori Bulunamadı.");

            return ApiResponses<string>.Success(categoryName);
        }

    }
}
