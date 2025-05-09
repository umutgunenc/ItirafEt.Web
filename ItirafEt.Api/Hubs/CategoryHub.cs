using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class CategoryHub :Hub
    {

        public Task JoinCategoryInfoChangedGroup() => Groups.AddToGroupAsync(Context.ConnectionId, $"CategoryInfoChanged");
        public Task JoinCategoryPostCountChangedGroup() => Groups.AddToGroupAsync(Context.ConnectionId, $"CategoryPostCountChanged");

        //public async Task NotifyActiveCategoryInformationsChanged(CategoryDto Category)
        //{
        //    await Clients.All.SendAsync("ActiveCategoryInformationsChanged", Category);
        //}

    }
}
