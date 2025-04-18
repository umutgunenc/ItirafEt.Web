using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class CategoryHub :Hub
    {
        public async Task NotifyActiveCategoryInformationsChanged()
        {
            await Clients.All.SendAsync("ActiveCategoryInformationsChanged");
        }

        
    }
}
