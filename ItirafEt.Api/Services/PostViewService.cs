using System.Xml;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class PostViewService
    {
        private readonly dbContext _context;
        private readonly PostViewHubService _postViewHubService;


        public PostViewService(dbContext context, PostViewHubService postViewHubService)
        {
            _context = context;
            _postViewHubService = postViewHubService;
        }

        public async Task<ApiResponses> ReadPostAsync(int postId, Guid? userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return ApiResponses.Success();

            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == postId && x.IsActive);
            if (post == null)
                return ApiResponses.Fail("Gönderi Bulunamadı.");

            if (post.UserId == userId)
                return ApiResponses.Success();

            var didUserReadPostBefore = await _context
                                                .UserReadPosts
                                                .AnyAsync(x => x.PostId == postId && x.UserId == userId);

            if (didUserReadPostBefore)
                return ApiResponses.Success();

            var userReadPost = new UserReadPost
            {
                PostId = postId,
                UserId = (Guid)userId,
                ReadDate = DateTime.Now
            };
            await _context.UserReadPosts.AddAsync(userReadPost);
            await _context.SaveChangesAsync();

            var postViewerDto = new PostViewersViewModel
            {
                PostViewerAge = DateTime.Now.Year - user.BirthDate.Year,
                PostViewerGenderId = user.GenderId,
                PostViewerUserName = user.UserName,
                PostViewerUserId = user.Id,
                PostViewerUserProfileImageUrl = user.ProfilePictureUrl,
                ReadDate = userReadPost.ReadDate
            };

            await _postViewHubService.PostViewedAsync(postId, postViewerDto);
            await _postViewHubService.UpdatePostViewCountAsync(post.CategoryId, postId);
            await _postViewHubService.PostViewedAnonymousAsync(postId);
                

            return ApiResponses.Success();
        }

        public async Task<ApiResponses<List<PostViewersViewModel>>> GetPostsViewersAsync(int postId)
        {
            var postViewers = await _context.UserReadPosts
                .AsNoTracking()
                .Include(urp => urp.User)
                .Where(urp => urp.PostId == postId)
                .Select(urp => new PostViewersViewModel
                {
                    PostViewerAge = DateTime.Now.Year - urp.User.BirthDate.Year,
                    PostViewerGenderId = urp.User.GenderId,
                    PostViewerUserName = urp.User.UserName,
                    PostViewerUserId = urp.User.Id,
                    PostViewerUserProfileImageUrl = urp.User.ProfilePictureUrl,
                    ReadDate = urp.ReadDate

                })
                .ToListAsync();

            return ApiResponses<List<PostViewersViewModel>>.Success(postViewers);
        }

        public async Task<ApiResponses<int>> GetPostViewCountAsync(int postId)
        {
            var postViewCount = await _context.UserReadPosts
                .AsNoTracking()
                .Where(urp => urp.PostId == postId)
                .CountAsync();

            return ApiResponses<int>.Success(postViewCount);
        }
    }
}
