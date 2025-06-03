using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class PostViewHub :Hub
    {
        public Task JoinPostPostViewCountGroup(int postId) => Groups.AddToGroupAsync(Context.ConnectionId, $"postPostViewed-{postId}");
        public Task JoinCategoryPostViewCountGroup(int categoryId) => Groups.AddToGroupAsync(Context.ConnectionId, $"categoryPostViewed-{categoryId}");

        //public async Task NotifyPostRead(int categoryId, int postId, int postViewCount)
        //{
        //    await Clients.Group($"post-{postId}")
        //        .SendAsync("PostRead", postId, postViewCount, categoryId);

        //    await Clients.Group($"category-{categoryId}")
        //        .SendAsync("PostRead", postId, postViewCount, categoryId);
        //}

    }
}
