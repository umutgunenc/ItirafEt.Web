using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace ItirafEt.SharedComponents.Services
{
    public abstract class SignalRServiceBase : ISignalRService, IAsyncDisposable
    {
        protected readonly ConcurrentDictionary<HubConstants.HubType, HubConnection> _connections = new();

        protected HubConnection? GetConnection(HubConstants.HubType hubType)
        {
            return _connections.TryGetValue(hubType, out var connection) ? connection : null;
        }

        protected bool IsConnected(HubConstants.HubType hubType)
        {
            return _connections.TryGetValue(hubType, out var connection) &&
                   connection.State == HubConnectionState.Connected;
        }

        public abstract HubConnection? ConfigureHubConnection(HubConstants.HubType hubType);

        public async Task StopAsync(HubConstants.HubType hubType)
        {
            if (_connections.TryGetValue(hubType, out var connection))
                await connection.StopAsync();
        }

        public async Task DisposeAsync(HubConstants.HubType hubType)
        {
            if (_connections.TryRemove(hubType, out var connection))
                await connection.DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var connection in _connections.Values)
                await connection.DisposeAsync();

            _connections.Clear();
        }
    }
}
