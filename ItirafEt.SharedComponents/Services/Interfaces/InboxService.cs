
using ItirafEt.Shared;
using ItirafEt.Shared.Services;
using ItirafEt.Shared.ViewModels;
using ItirafEt.SharedComponents.Apis;

namespace ItirafEt.SharedComponents.ClientServices
{
    public class InboxService : IInboxService
    {
        private Task? _initializationTask;

        private List<InboxItemViewModel> _conversations = new();
        public bool _isInitialized = false;
        public string? _errorMessage { get; set; }
        public bool _haveError { get; set; }
        private readonly IMessageApi MessageApi;
        private readonly IStorageService _storageService;
        public event Func<Task>? MessageReaded;
        public event Func<Task>? NewMessageRecived;
        public event Func<Task>? NewMessageRecivedInInboxPage;


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

        public async Task<List<InboxItemViewModel>?> GetUserConversationsAsync()
        {
            return await _storageService.GetItemAsync<List<InboxItemViewModel>>("UserConversations", false);
        }

        public async Task RemoveConversationsFromLocalStorageAsync()
        {
            await _storageService.RemoveItemAsync("UserConversations", false);
        }

        public async Task SetUserConversationsToLocalStorageAsync(List<InboxItemViewModel> Conversations)
        {
            await _storageService.SetItemAsync("UserConversations", Conversations,false);
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

        public async Task NotifyMessageReadedAsync()
        {
            if (MessageReaded is not null)
                await MessageReaded.Invoke();
        }

        public async Task UpdateInboxItemsAfterUnreadMessageReadedAsync(Guid conversationId)
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

        public async Task NotifyNewMessageRecivedAsync()
        {
            if (NewMessageRecived is not null)
                await NewMessageRecived.Invoke();
        }

        public async Task UpdateInboxItemsAfterNewMessageRecivedAsync(InboxItemViewModel conversation)
        {
            var conversations = await GetUserConversationsAsync();

            if (conversations?.Count != 0)
                await RemoveConversationsFromLocalStorageAsync();

            var item = conversations.FirstOrDefault(i => i.ConversationId == conversation.ConversationId);
            if (item != null)
            {
                item.LastMessagePrewiew = conversation.LastMessagePrewiew;
                item.LastMessageDate = conversation.LastMessageDate;
                item.UnreadMessageCount++;
            }

            await SetUserConversationsToLocalStorageAsync(conversations);
        }
        public async Task NotifyNewMessageRecivedInInboxPageAsync()
        {
            if (NewMessageRecivedInInboxPage is not null)
                await NewMessageRecivedInInboxPage.Invoke();
        }
        public async Task UpdateInboxItemsAfterNewMessageRecivedInInboxPageAsync(InboxItemViewModel conversation)
        {
            var conversations = await GetUserConversationsAsync();

            if (conversations?.Count != 0)
                await RemoveConversationsFromLocalStorageAsync();

            var item = conversations.FirstOrDefault(i => i.ConversationId == conversation.ConversationId);
            if (item != null)
            {
                item.LastMessagePrewiew = conversation.LastMessagePrewiew;
                item.LastMessageDate = conversation.LastMessageDate;
            }

            await SetUserConversationsToLocalStorageAsync(conversations);
        }
    }
}
