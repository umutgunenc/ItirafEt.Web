using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared.Models;
using ItirafEt.Shared.ViewModels;
using ItirafEt.SharedComponents.Services;
using Microsoft.AspNetCore.SignalR.Client;
using static ItirafEt.Shared.Models.HubConstants;
using static ItirafEt.SharedComponents.Helpers.PageNameConstants;

public class SignalRInboxService : IAsyncDisposable
{
    private HubConnection? _connection;
    private CancellationTokenSource _cts = new();
    private readonly ISignalRService _signalRServices;
    private Guid _currentUserId;

    public event Func<InboxItemViewModel, Task>? NewInboxMessage;
    public event Func<Guid, Guid, Task>? MessageRead;

    public SignalRInboxService(ISignalRService signalRServices)
    {
        _signalRServices = signalRServices;
    }

    public async Task InitializeAsync(Guid currentUserId)
    {
        if (_cts.IsCancellationRequested)
            return;
        if (_connection != null &&
            _connection.State == HubConnectionState.Connected &&
            _currentUserId == currentUserId)
            return;

        if (_cts.IsCancellationRequested)
            return;
        // Eğer eski connection varsa temizle
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
        }

        if (_cts.IsCancellationRequested)
            return;
        _currentUserId = currentUserId;
        _connection = await _signalRServices.ConfigureHubConnectionAsync(HubType.Message,PageType.Layout);

        if(_cts.IsCancellationRequested)
            return;

        _connection.On<Guid, InboxItemViewModel>(
            "NewMessageForInboxAsync",
            (_, model) => NewInboxMessage?.Invoke(model) ?? Task.CompletedTask);

        _connection.On<Guid, Guid>(
            "MessageReadByCurrentUserAsync",
            (uid, convId) => MessageRead?.Invoke(uid, convId) ?? Task.CompletedTask);

        if (_cts.IsCancellationRequested)
            return;

        await _connection.StartAsync(_cts.Token);

        // Aynı connection ile iki gruba katılıyoruz
        await _connection.SendAsync("JoinInboxGroup", currentUserId, _cts.Token);
        await _connection.SendAsync("JoinMessageReadGroup", currentUserId, _cts.Token);
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        if (_connection != null)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _signalRServices.StopAsync(PageType.Layout,HubType.Message);
            await _signalRServices.DisposeAsync(PageType.Layout, HubType.Message);
            _connection = null;
        }
    }
}
