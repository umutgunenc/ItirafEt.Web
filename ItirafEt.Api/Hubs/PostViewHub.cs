using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class PostViewHub :Hub
    {
        public Task JoinPostPostCountGroup(int postId) => Groups.AddToGroupAsync(Context.ConnectionId, $"post-{postId}");
        public Task JoinCategoryPostCountGroup(int categoryId) => Groups.AddToGroupAsync(Context.ConnectionId, $"category-{categoryId}");

        public async Task NotifyPostRead(int postId, int postViewCount)
        {
            await Clients.Group($"post-{postId}")
                .SendAsync("PostRead", postId, postViewCount);

            //var categoryId = await GetCategoryIdFromPostId(postId);

            //await Clients.Group($"category-{categoryId}")
            //    .SendAsync("PostRead", postId, postViewCount);
        }

        private Task<int> GetCategoryIdFromPostId(int postId)
        {
            throw new NotImplementedException();
        }
    }
}

       