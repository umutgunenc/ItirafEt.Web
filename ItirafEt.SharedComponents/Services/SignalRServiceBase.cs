using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Models;
using ItirafEt.Shared.Services;
using ItirafEt.SharedComponents.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using static ItirafEt.Shared.Models.HubConstants;
using static ItirafEt.SharedComponents.Helpers.PageNameConstants;

namespace ItirafEt.SharedComponents.Services
{
    public abstract class SignalRServiceBase : ISignalRService
    {
        protected readonly ConcurrentDictionary<Guid, HubConnection> _connections = new();
        protected readonly IStorageService _storageService;

        protected SignalRServiceBase(IStorageService storageService)
        {
            _storageService = storageService;
        }

        protected async Task<HubConnection?> GetConnectionAsync(PageType pageType, HubType hubType)
        {
            var connectionKey = await GetConnectionIdAsync(pageType, hubType);
            if (!connectionKey.HasValue)
                return null;

            _connections.TryGetValue(connectionKey.Value, out var connection);
            return connection;
        }

        protected bool IsConnected(HubConnection? connection)
        {
            return connection?.State == HubConnectionState.Connected;
        }

        public abstract Task<HubConnection?> ConfigureHubConnectionAsync(HubType hubType, PageType pageType);

        public async Task StopAsync(PageType pageType, HubType hubType)
        {
            var key = await GetConnectionIdAsync(pageType, hubType);
            if (key.HasValue && _connections.TryGetValue(key.Value, out var connection))
            {
                if (connection != null)
                    await connection.StopAsync();
            }
        }


        public async Task DisposeAsync(PageType pageType, HubType hubType)
        {
            var key = await GetConnectionIdAsync(pageType, hubType);
            if (key.HasValue && _connections.TryRemove(key.Value, out var connection))
            {
                if (connection != null)
                {
                    await connection.DisposeAsync();
                    await _storageService.RemoveItemAsync(GetKey(pageType, hubType),false);
                }
            }
        }

        private async Task<Guid?> GetConnectionIdAsync(PageType pageType, HubType hubType)
        {

            var key = GetKey(pageType, hubType);
            return await _storageService.GetItemAsync<Guid?>(key, false);
        }

        protected string GetKey(PageType pageType, HubType hubType)
        {
            return $"SignalR-{PageNameConstants.GetPageName(pageType)}-Page/{HubConstants.GetHubUrl(hubType)}-Hub";
        }


        public async Task DisposeAllAsync()
        {
            var allKeys = new List<string>(_connections.Count);

            foreach (var pageType in Enum.GetValues<PageType>())
            {
                foreach (var hubType in Enum.GetValues<HubType>())
                {
                    var key = GetKey(pageType, hubType);
                    var connectionId = await _storageService.GetItemAsync<Guid?>(key, false);
                    if (connectionId.HasValue)
                    {
                        if (_connections.TryRemove(connectionId.Value, out var connection))
                        {
                            await connection.StopAsync();
                            await connection.DisposeAsync();
                            await _storageService.RemoveItemAsync(key, false);
                        }
                    }
                }
            }
            _connections.Clear();
        }
    }
}
