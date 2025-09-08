using System.Text.RegularExpressions;
using ItirafEt.Api.Data.Entities;
using Microsoft.AspNetCore.SignalR;

namespace ItirafEt.Api.Hubs
{
    public class BanUserHub : Hub
    {
        public Task JoinBanUserGroup(string bannedUserId) => Groups.AddToGroupAsync(Context.ConnectionId, $"userId-{bannedUserId}");

    }
}
