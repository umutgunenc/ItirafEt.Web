using System.Xml;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class PostViewService
    {
        private readonly dbContext _context;
        //private readonly IHubContext<PostViewHub> _postViewHub;
        private readonly PostViewHubService _postViewHubService;


        public PostViewService(dbContext context, PostViewHubService postViewHubService)
        {
            _context = context;
            _postViewHubService = postViewHubService;
        }

        public async Task<ApiResponses<PostViewersDto?>> ReadPostAsync(int postId, Guid? userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return ApiResponses<PostViewersDto?>.Success(null);

            var post = await _context.Posts
                .FirstOrDefaultAsync(x => x.Id == postId && x.IsActive);
            if (post == null)
                return ApiResponses<PostViewersDto?>.Fail("Gönderi Bulunamadı.");

            if (post.UserId == userId)
                return ApiResponses<PostViewersDto?>.Success(null);

            var didUserReadPostBefore = await _context
                                                .UserReadPosts
                                                .AnyAsync(x => x.PostId == postId && x.UserId == userId);

            if (didUserReadPostBefore)
                return ApiResponses<PostViewersDto?>.Success(null);

            var userReadPost = new UserReadPost
            {
                PostId = postId,
                UserId = (Guid)userId,
                ReadDate = DateTime.Now
            };
            await _context.UserReadPosts.AddAsync(userReadPost);
            await _context.SaveChangesAsync();

            ////
            ///
            var postViewerDto = new PostViewersDto
            {
                PostId = postId,
                PostTitle = post.Title,
                PostViewerAge = DateTime.Now.Year - user.BirthDate.Year,
                PostViewerGenderId = user.GenderId,
                PostViewerUserName = user.UserName,
                PostViewerUserId = user.Id,
                PostViewerUserProfileImageUrl = user.ProfilePictureUrl,
                ReadDate = userReadPost.ReadDate
            };

            await _postViewHubService.PostViewedAsync(postId, postViewerDto);
            await _postViewHubService.UpdatePostViewCountAsync(post.CategoryId, postId);

            return ApiResponses<PostViewersDto?>.Success(postViewerDto);

            //var postCategoryIdAndViewCount = await _context.Posts
            //    .Include(p => p.Readers)
            //    .Where(p => p.Id == postId)
            //    .Select(p => new
            //    {
            //        CategoryId = p.CategoryId,
            //        ViewCount = p.Readers.Count
            //    })
            //    .FirstOrDefaultAsync();

            //int postCategoryId = postCategoryIdAndViewCount.CategoryId;
            //int postViewCount = postCategoryIdAndViewCount.ViewCount;

            //await _postViewHub.Clients
            //    .Group($"post-{postId}")
            //    .SendAsync("PostRead", postCategoryId, postId, postViewCount);

            //await _postViewHub.Clients
            //    .Group($"category-{postCategoryId}")
            //    .SendAsync("PostRead", postCategoryId, postId, postViewCount);

            //return ApiResponses<(int?, int?)>.Success((postViewCount, postId));
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
                    PostViewerAge = DateTime.Now.Year - urp.User.BirthDate.Year,
                    PostViewerGenderId = urp.User.GenderId,
                    PostViewerUserName = urp.User.UserName,
                    PostViewerUserId = urp.User.Id,
                    PostViewerUserProfileImageUrl = urp.User.ProfilePictureUrl,
                    ReadDate = urp.ReadDate

                })
                .ToListAsync();

            return ApiResponses<List<PostViewersDto>>.Success(postViewers);
        }

        //public async Task<ApiResponses<int>> GetPostReadCountAsync(int postId)
        //{
        //    var readCount = await _context.UserReadPosts
        //        .CountAsync(p => p.PostId == postId);

        //    return ApiResponses<int>.Success(readCount);
        //}

        ////TODO : kullanıcının okumuş olduğu gönderileri işaretlemek için
        //public async Task<ApiResponses<List<UserViewedPostsDto>>> GetUserViewedPostInfoAsync(Guid userId)
        //{
        //    // Kullanıcının görüntülediği postları getirir
        //    var readPosts = await _context.UserReadPosts
        //        .Include(urp => urp.Post)
        //        .ThenInclude(p => p.User)
        //        .Where(urp => urp.UserId == userId && urp.Post.IsActive)
        //        .Select(urp => new UserViewedPostsDto
        //        {
        //            PostId = urp.PostId,
        //            PostTitle = urp.Post.Title,
        //            PostCreatorUserName = urp.Post.User.UserName,
        //            PostCreatorUserId = urp.Post.UserId,
        //            PostCreatorUserProfileImageUrl = urp.Post.User.ProfilePictureUrl,
        //            PostCreatorUserAge = urp.Post.User.BirthDate.Year - DateTime.Now.Year,
        //            PostCreatorUserGenderId = urp.Post.User.GenderId,
        //            ReadDate = urp.ReadDate
        //        })
        //        .ToListAsync();

        //    if (readPosts == null || readPosts.Count == 0)
        //        return ApiResponses<List<UserViewedPostsDto>>.Fail("Okunan gönderi bulunamadı");
        //    return ApiResponses<List<UserViewedPostsDto>>.Success(readPosts);
        //}

        ////TODO : gönderiyi okuyan kullanıcıları işaretlemek için


        //public async Task<ApiResponses<bool>> DidUserReadPostAsync(int postId, Guid userId)
        //{
        //    var reponse = await _context.UserReadPosts
        //        .AnyAsync(x => x.PostId == postId && x.UserId == userId);

        //    return ApiResponses<bool>.Success(reponse);
        //}
    }
}
