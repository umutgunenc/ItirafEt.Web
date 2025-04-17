using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class ReactionHub : Hub
    {
        public async Task NotifyPostLikedOrDisliked()
        {
            await Clients.All.SendAsync("LikedOrDisliked");
        }
    }
    
}
