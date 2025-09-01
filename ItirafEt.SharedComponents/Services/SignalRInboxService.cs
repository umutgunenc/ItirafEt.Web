using ItirafEt.Shared.Models;
using ItirafEt.Shared.ViewModels;
using ItirafEt.SharedComponents.Services;
using Microsoft.AspNetCore.SignalR.Client;

public class SignalRInboxService : IAsyncDisposable
{
    private HubConnection? _connection;
    private readonly ISignalRService _signalRServices;
    private Guid _currentUserId;

    public event Func<InboxItemViewModel, Task>? NewInboxMessage;
    public event Func<Guid, Guid, Task>? MessageRead;

    public SignalRInboxService(ISignalRService signalRServices)
        => _signalRServices = signalRServices;

    public async Task InitializeAsync(Guid currentUserId)
    {
        if (_connection != null &&
            _connection.State == HubConnectionState.Connected &&
            _currentUserId == currentUserId)
            return;

        // Eğer eski connection varsa temizle
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
        }

        _currentUserId = currentUserId;
        _connection = _signalRServices.ConfigureHubConnection(HubConstants.HubType.Message);

        _connection.On<Guid, InboxItemViewModel>(
            "NewMessageForInboxAsync",
            (_, model) => NewInboxMessage?.Invoke(model) ?? Task.CompletedTask);

        _connection.On<Guid, Guid>(
            "MessageReadByCurrentUserAsync",
            (uid, convId) => MessageRead?.Invoke(uid, convId) ?? Task.CompletedTask);

        await _connection.StartAsync();

        // Aynı connection ile iki gruba katılıyoruz
        await _connection.SendAsync("JoinInboxGroup", currentUserId);
        await _connection.SendAsync("JoinMessageReadGroup", currentUserId);
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
