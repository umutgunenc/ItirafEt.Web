using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.HubServices
{
    public class BanUserHubService
    {
        private readonly IHubContext<BanUserHub> _banUserHub;

        public BanUserHubService(IHubContext<BanUserHub> banUserHub)
        {
            _banUserHub = banUserHub;
        }

        public async Task UserBannedAsync(Guid userBannedUserId)
        {
            await _banUserHub.Clients
                .Group($"userId-{userBannedUserId}")
                .SendAsync("UserBannedAsync", userBannedUserId);
        }
    }
}