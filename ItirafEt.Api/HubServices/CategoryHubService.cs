using ItirafEt.Api.Hubs;
using ItirafEt.Shared.ViewModels;
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

        public async Task CategoryInfoChangedAsync(CategoryViewModel model)
        {
            await _categoryHub.Clients
                .Group($"CategoryInfoChanged")
                .SendAsync("CategoryInfoChangedAsync", model);
        }

        public async Task CategoryPostCountChangedAsync(int categoryId, bool isPostAdded)
        {
            await _categoryHub.Clients
                .Group($"CategoryPostCountChanged")
                .SendAsync("CategoryPostCountChangedAsync", categoryId, isPostAdded);
        }
    }
}
