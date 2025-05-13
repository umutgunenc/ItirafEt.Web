using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
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
        public async Task PostViewedAsync(int postId, PostViewersDto dto)
        {
            await _hubContext.Clients
                .Group($"postPostViewed-{postId}")
                .SendAsync("PostViewedAsync", postId, dto);
        }

        public async Task UpdatePostViewCountAsync(int categoryId, int postId)
        {
            await _hubContext.Clients
                .Group($"categoryPostViewed-{categoryId}")
                .SendAsync("UpdatePostViewCountAsync", postId);
        }
    }
}
