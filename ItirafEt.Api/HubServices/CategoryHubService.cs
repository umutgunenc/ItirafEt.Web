using ItirafEt.Api.Hubs;
using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.HubServices
{
    public class CategoryHubService
    {
        private readonly IHubContext<CategoryHub> _categoryHub;

        public CategoryHubService(IHubContext<CategoryHub> categoryHub)
        {
            _categoryHub = categoryHub;
        }

        public async Task CategoryInfoChangedAsync(CategoryDto categoryDto)
        {
            await _categoryHub.Clients
                .Group($"CategoryInfoChanged")
                .SendAsync("CategoryInfoChangedAsync", categoryDto);
        }

        public async Task CategoryPostCountChanged(int categoryId, bool isPostAdded)
        {
            await _categoryHub.Clients
                .Group($"CategoryPostCountChanged")
                .SendAsync("CategoryPostCountChanged", categoryId, isPostAdded);
        }
    }
}
