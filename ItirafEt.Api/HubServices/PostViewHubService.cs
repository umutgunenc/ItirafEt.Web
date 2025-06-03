using ItirafEt.Api.Hubs;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.HubServices
{
    public class PostViewHubService
    {
        private readonly IHubContext<PostViewHub> _hubContext;
        public PostViewHubService(IHubContext<PostViewHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task PostViewedAsync(int postId, PostViewersViewModel model)
        {
            await _hubContext.Clients
                .Group($"postPostViewed-{postId}")
                .SendAsync("PostViewedAsync", postId, model);
        }
        public async Task PostViewedAnonymousAsync(int postId)
        {
            await _hubContext.Clients
                .Group($"postPostViewed-{postId}")
                .SendAsync("PostViewedAnonymousAsync", postId);
        }

        public async Task UpdatePostViewCountAsync(int categoryId, int postId)
        {
            await _hubContext.Clients
                .Group($"categoryPostViewed-{categoryId}")
                .SendAsync("UpdatePostViewCountAsync", postId);
        }


    }
}
