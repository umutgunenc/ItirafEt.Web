using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class PostViewService
    {
        private readonly dbContext _context;
        private readonly IHubContext<PostViewHub> _postViewHub;

        public PostViewService(dbContext context, IHubContext<PostViewHub> postViewHub)
        {
            _context = context;
            _postViewHub = postViewHub;
        }

        public async Task<ApiResponses<(int?,int?)>> ReadPostAsync(int postId, Guid? userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return ApiResponses<(int?, int?)>.Fail("Kullanıcı bulunamadı.");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return ApiResponses<(int?, int?)>.Fail("Gönderi Bulunamadı.");

            if (!post.IsActive)
                return ApiResponses<(int?, int?)>.Fail("Gönderi aktif değil.");

            if (post.UserId == userId)
                return ApiResponses<(int?, int?)>.Fail("Kendi gönderinizi okuyamazsınız.");

            var didUserReadPostBefore = await _context.UserReadPosts
                .AnyAsync(x => x.PostId == postId && x.UserId == userId);

            if (didUserReadPostBefore)
                return ApiResponses<(int?, int?)>.Success((null,null));

            var userReadPost = new UserReadPost
            {
                PostId = postId,
                UserId = (Guid)userId,
                ReadDate = DateTime.Now
            };
            await _context.UserReadPosts.AddAsync(userReadPost);
            await _context.SaveChangesAsync();

            var postCategoryIdAndViewCount = await _context.Posts
                .Include(p => p.Readers)
                .Where(p => p.Id == postId)
                .Select(p => new {
                    CategoryId = p.CategoryId,
                    ViewCount = p.Readers.Count
                })
                .FirstOrDefaultAsync();

            int postCategoryId = postCategoryIdAndViewCount.CategoryId;
            int postViewCount = postCategoryIdAndViewCount.ViewCount;

            await _postViewHub.Clients
                .Group($"post-{postId}")
                .SendAsync("PostRead", postCategoryId, postId, postViewCount);

            await _postViewHub.Clients
                .Group($"category-{postCategoryId}")
                .SendAsync("PostRead", postCategoryId, postId, postViewCount);

            return ApiResponses<(int?, int?)>.Success((postViewCount, postId));
        }

        public async Task<ApiResponses<int>> GetPostReadCountAsync(int postId)
        {
            var readCount = await _context.UserReadPosts
                .CountAsync(p => p.PostId == postId);

            return ApiResponses<int>.Success(readCount);
        }

        public async Task<ApiResponses<List<UserViewedPostsDto>>> GetUserViewedPostInfoAsync(Guid userId)
        {
            // Kullanıcının görüntülediği postları getirir
            var readPosts = await _context.UserReadPosts
                .Include(urp => urp.Post)
                .ThenInclude(p => p.User)
                .Where(urp => urp.UserId == userId && urp.Post.IsActive)
                .Select(urp => new UserViewedPostsDto
                {
                    PostId = urp.PostId,
                    PostTitle = urp.Post.Title,
                    PostCreatorUserName = urp.Post.User.UserName,
                    PostCreatorUserId = urp.Post.UserId,
                    PostCreatorUserProfileImageUrl = urp.Post.User.ProfilePictureUrl,
                    PostCreatorUserAge = urp.Post.User.BirthDate.Year - DateTime.Now.Year,
                    PostCreatorUserGenderId = urp.Post.User.GenderId,
                    ReadDate = urp.ReadDate
                })
                .ToListAsync();

            if (readPosts == null || readPosts.Count == 0)
                return ApiResponses<List<UserViewedPostsDto>>.Fail("Okunan gönderi bulunamadı");
            return ApiResponses<List<UserViewedPostsDto>>.Success(readPosts);
        }

        public async Task<ApiResponses<List<PostViewersDto>>> GetPostsViewersAsync(int postId)
        {
            // Postları görüntüleyen kullanıcıların bilgilerini getirir
            var postViewers = await _context.UserReadPosts
                .Include(urp => urp.User)
                .Where(urp => urp.PostId == postId)
                .Select(urp => new PostViewersDto
                {
                    PostId = postId,
                    PostTitle = urp.Post.Title,
                    PostViewerAge = urp.User.BirthDate.Year - DateTime.Now.Year,
                    PostViewerGenderId = urp.User.GenderId,
                    PostViewerUserName = urp.User.UserName,
                    PostViewerUserId = urp.User.Id,
                    PostViewerUserProfileImageUrl = urp.User.ProfilePictureUrl,
                    ReadDate = urp.ReadDate

                })
                .ToListAsync();
            if (postViewers == null || postViewers.Count == 0)
                return ApiResponses<List<PostViewersDto>>.Fail("Gönderi kimse okumadı.");
            return ApiResponses<List<PostViewersDto>>.Success(postViewers);
        }

        public async Task<ApiResponses<bool>> DidUserReadPostAsync(int postId, Guid userId)
        {
            var reponse = await _context.UserReadPosts
                .AnyAsync(x => x.PostId == postId && x.UserId == userId);

            return ApiResponses<bool>.Success(reponse);
        }
    }
}