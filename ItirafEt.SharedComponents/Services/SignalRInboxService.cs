using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Models;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;

namespace ItirafEt.SharedComponents.Services
{
    public class SignalRInboxService
    {
        private HubConnection? _connectionNewMessage;
        private HubConnection? _connectionReadMessage;
        private HubConnection? _connection;
        private readonly ISignalRService _signalRServices;

        public SignalRInboxService(ISignalRService signalRServices)
        {
            _signalRServices = signalRServices;
        }

        private Guid _currentUserId;

        public event Func<InboxViewModel, Task>? NewInboxMessage;
        public event Func<Guid, Guid, Task>? MessageRead;
        public async Task StartAsync(Guid currentUserId)
        {
            if (_connection is not null) return;

            _currentUserId = currentUserId;
            //_connection = new HubConnectionBuilder()
            //    .WithUrl("https://localhost:7292/messagehub")
            //    .WithAutomaticReconnect()
            //    .Build();
            _signalRServices.ConfigureHubConnection(HubConstants.HubType.Message);
            _connection = _signalRServices.GetConnection(HubConstants.HubType.Message);
            //_connection = new HubConnectionBuilder()
            //    .WithUrl("http://localhost:7292/messagehub")
            //    .WithAutomaticReconnect()
            //    .Build();

            _connection.On<Guid, InboxViewModel>("NewMessageForInboxAsync", (_, model) =>
                NewInboxMessage?.Invoke(model) ?? Task.CompletedTask
            );

            _connection.On<Guid, Guid>("MessageReadByCurrentUserAsync", (uid, convId) =>
                    MessageRead?.Invoke(uid, convId) ?? Task.CompletedTask
            );

            await _connection.StartAsync();
            await _connection.SendAsync("JoinInboxGroup", currentUserId);
            await _connection.SendAsync("JoinMessageReadGroup", currentUserId);

        }

        public async Task StartReadMessageAsync(Guid currentUserId)
        {
            if (_connectionReadMessage is not null) return;

            _currentUserId = currentUserId;
            //_connectionReadMessage = new HubConnectionBuilder()
            //    .WithUrl("https://localhost:7292/messagehub")
            //    .WithAutomaticReconnect()
            //    .Build();           
            _signalRServices.ConfigureHubConnection(HubConstants.HubType.Message);
            _connectionReadMessage = _signalRServices.GetConnection(HubConstants.HubType.Message);

            //_connectionReadMessage = new HubConnectionBuilder()
            //    .WithUrl("http://10.0.2.2:7292/messagehub")
            //    .WithAutomaticReconnect()
            //    .Build();

            _connectionReadMessage.On<Guid, Guid>("MessageReadByCurrentUserAsync", (uid, convId) =>
                MessageRead?.Invoke(uid, convId) ?? Task.CompletedTask
            );

            await _connectionReadMessage.StartAsync();
            await _connectionReadMessage.SendAsync("JoinMessageReadGroup", currentUserId);
        }

        public async ValueTask DisposeNewMessageAsync()
        {
            if (_connectionNewMessage != null)
            {
                await _connectionNewMessage.StopAsync();
                await _connectionNewMessage.DisposeAsync();
                _connectionNewMessage = null;
            }
        }       
        
        public async ValueTask DisposeReadMessageAsync()
        {
            if (_connectionReadMessage != null)
            {
                await _connectionReadMessage.StopAsync();
                await _connectionReadMessage.DisposeAsync();
                _connectionReadMessage = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
        }
    }
}
