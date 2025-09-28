using ItirafEt.Shared.Models;
using ItirafEt.Shared.Services;
using ItirafEt.SharedComponents.Helpers;
using ItirafEt.SharedComponents.Services;
using Microsoft.AspNetCore.SignalR.Client;
using static ItirafEt.Shared.Models.HubConstants;
using static ItirafEt.SharedComponents.Helpers.PageNameConstants;

namespace ItirafEt.Web.Services
{
    public class WebSignalRService : SignalRServiceBase
    {
        public WebSignalRService(IStorageService storageService) : base(storageService)
        {
        }

        public override async Task<HubConnection?> ConfigureHubConnectionAsync(HubType hubType, PageType pageType)
        {
            var connection = await GetConnectionAsync(pageType, hubType);
            if (connection is not null && IsConnected(connection)) {
                await StopAsync(pageType, hubType);
                await DisposeAsync(pageType, hubType);
            }


            var baseUrl = ApiBaseUrl.BaseUrl;

            var hubUrl = $"{baseUrl}{HubConstants.GetHubUrl(hubType)}";

            var newConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            var connectionId = Guid.NewGuid();
            _connections.TryAdd(connectionId, newConnection);

            var key = GetKey(pageType, hubType);
            await _storageService.SetItemAsync(key, connectionId);

            return newConnection;
        }
    }
}
