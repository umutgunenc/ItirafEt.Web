using ItirafEt.Api.Data;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class UserProfileService
    {
        private readonly dbContext _context;

        public UserProfileService(dbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponses<UserProfileViewModel>> GetUserProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && !u.IsBanned);

            if (user == null)
                return ApiResponses<UserProfileViewModel>.Fail("Kullanıcı Bulunamadı");

            if(user.IsProfilePrivate)
                return ApiResponses<UserProfileViewModel>.Fail("Kullanıcının profili gizli.");


            var userProfileViewModel = new UserProfileViewModel
            {
                BirthDate = user.BirthDate,
                UserName = user.UserName,
                GenderId = user.GenderId,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return ApiResponses<UserProfileViewModel>.Success(userProfileViewModel);

        }

        public async Task<ApiResponses<UserPostsViewModel>> GetUserPostsDateOrderedAsync(Guid userId, int size, int page)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && !u.IsBanned);

            if (user == null)
                return ApiResponses<UserPostsViewModel>.Fail("Kullanıcı Bulunamadı.");

            var totalPosts = await _context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeletedByUser && !p.IsDeletedByAdmin && p.Category.isActive)
                .CountAsync(p => p.UserId == userId);

            if (totalPosts == 0)
            {
                var emptyUserPostsViewModel = new UserPostsViewModel();
                return ApiResponses<UserPostsViewModel>.Success(emptyUserPostsViewModel);
            }

            var posts = await _context.Posts
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeletedByUser && !p.IsDeletedByAdmin && p.Category.isActive)
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
                    PostUpdatedDate = p.UpdatedDate
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


        public async Task<ApiResponses<UserPostsViewModel>> GetUserPostsLikeCountOrderedAsync(Guid userId, int size, int page)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && !u.IsBanned);

            if (user == null)
                return ApiResponses<UserPostsViewModel>.Fail("Kullanıcı Bulunamadı.");

            var totalPosts = await _context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeletedByUser && !p.IsDeletedByAdmin && p.Category.isActive)
                .CountAsync(p => p.UserId == userId);

            if (totalPosts == 0)
            {
                var emptyUserPostsViewModel = new UserPostsViewModel();
                return ApiResponses<UserPostsViewModel>.Success(emptyUserPostsViewModel);
            }

            var posts = await _context.Posts
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeletedByUser && !p.IsDeletedByAdmin && p.Category.isActive)
                .OrderByDescending(p => p.PostReactions
                        .Where(pr => pr.ReactionTypeId == (int)ReactionTypeEnum.Like)
                        .Count())
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
                    PostUpdatedDate = p.UpdatedDate

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


        public async Task<ApiResponses<UserPostsViewModel>> GetUserPostsReadCountOrderedAsync(Guid userId, int size, int page)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && !u.IsBanned);

            if (user == null)
                return ApiResponses<UserPostsViewModel>.Fail("Kullanıcı Bulunamadı.");

            var totalPosts = await _context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeletedByUser && !p.IsDeletedByAdmin && p.Category.isActive)
                .CountAsync(p => p.UserId == userId);

            if (totalPosts == 0)
            {
                var emptyUserPostsViewModel = new UserPostsViewModel();
                return ApiResponses<UserPostsViewModel>.Success(emptyUserPostsViewModel);
            }

            var posts = await _context.Posts
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeletedByUser && !p.IsDeletedByAdmin && p.Category.isActive)
                .OrderByDescending(p => p.Readers.Count)
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
                    PostUpdatedDate = p.UpdatedDate
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
