
using ItirafEt.Shared;
using ItirafEt.Shared.Services;
using ItirafEt.Shared.ViewModels;
using ItirafEt.SharedComponents.Apis;

namespace ItirafEt.SharedComponents.ClientServices
{
    public class InboxService : IInboxService
    {
        private Task? _initializationTask;

        private List<InboxViewModel> _conversations = new();
        public bool _isInitialized = false;
        public string? _errorMessage { get; set; }
        public bool _haveError { get; set; }
        private readonly IMessageApi MessageApi;
        private readonly IStorageService _storageService;
        public event Func<Task>? OnUnreadMessageChanged;
        public event Func<Task>? InboxItemsOrderChanged;


        public InboxService(IMessageApi messageApi, IStorageService storageService)
        {
            MessageApi = messageApi;
            _storageService = storageService;
        }

        public async Task InitializeAsync(Guid userId)
        {
            if (_isInitialized)
                return;

            if (_initializationTask != null)
            {
                await _initializationTask;
                return;
            }

            _initializationTask = InitializeInternalAsync(userId);
            await _initializationTask;
        }
        private async Task InitializeInternalAsync(Guid userId)
        {
            try
            {
                var response = await MessageApi.GetUserMessagesAsync(userId);
                if (response.IsSuccess)
                {
                    _conversations.Clear();
                    _conversations.AddRange(response.Data);
                    await SetUserConversationsToLocalStorageAsync(_conversations);
                    _haveError = false;
                    _isInitialized = true;
                }
                else
                {
                    _errorMessage = response.ErrorMessage;
                    _haveError = true;
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                _haveError = true;
            }
        }

        public async Task<List<InboxViewModel>?> GetUserConversationsAsync()
        {
            return await _storageService.GetItemAsync<List<InboxViewModel>>("UserConversations");
        }

        public async Task RemoveConversationsFromLocalStorageAsync()
        {
            await _storageService.RemoveItemAsync("UserConversations");
        }

        public async Task SetUserConversationsToLocalStorageAsync(List<InboxViewModel> Conversations)
        {
            await _storageService.SetItemAsync("UserConversations", Conversations);
        }

        public async Task ClearAsync()
        {
            _conversations.Clear();
            await RemoveConversationsFromLocalStorageAsync();
            _isInitialized = false;
            _haveError = false;
            _errorMessage = null;
            _initializationTask = null;
        }

        public async Task NotifyUnreadMessageChangedAsync()
        {
            if (OnUnreadMessageChanged is not null)
                await OnUnreadMessageChanged.Invoke();
        }

        public async Task UpdateUnreadMessageCountAsync(Guid conversationId)
        {
            var conversations = await GetUserConversationsAsync();
            if (conversations?.Count != 0)
                await RemoveConversationsFromLocalStorageAsync();

            var item = conversations.FirstOrDefault(i => i.ConversationId == conversationId);

            if (item != null)
                if (item.UnreadMessageCount < PageItemSize.Size)
                    item.UnreadMessageCount = 0;
                else
                    item.UnreadMessageCount = item.UnreadMessageCount - PageItemSize.Size;

            await SetUserConversationsToLocalStorageAsync(conversations);
        }

        public async Task NotifyInboxItemsOrderChangedAsync()
        {
            if (InboxItemsOrderChanged is not null)
                await InboxItemsOrderChanged.Invoke();
        }

        public async Task UpdateConversationOrderAsync(InboxViewModel conversation)
        {
            var conversations = await GetUserConversationsAsync();

            if (conversations?.Count != 0)
                await RemoveConversationsFromLocalStorageAsync();

            var item = conversations.FirstOrDefault(i => i.ConversationId == conversation.ConversationId);
            if (item != null)
            {
                item.LastMessagePrewiew = conversation.LastMessagePrewiew;
                item.LastMessageDate = conversation.LastMessageDate;
                item.UnreadMessageCount = conversation.UnreadMessageCount++;
            }

            await SetUserConversationsToLocalStorageAsync(conversations);
        }
    }
}
