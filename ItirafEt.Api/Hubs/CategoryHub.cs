using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class CategoryHub :Hub
    {
        public async Task NotifyCategoryChanged()
        {
            await Clients.All.SendAsync("CategoryChanged");
        }
    }
}
