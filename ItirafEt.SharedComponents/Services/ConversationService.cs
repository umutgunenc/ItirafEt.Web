
using ItirafEt.Shared.ViewModels;
using ItirafEt.SharedComponents.Apis;

namespace ItirafEt.SharedComponents.ClientServices
{
    public class ConversationService
    {
        private List<InboxViewModel> _conversations = new();
        private bool _isInitialized = false;

        private readonly IMessageApi MessageApi;

        public ConversationService(IMessageApi messageApi)
        {
            MessageApi = messageApi;
        }

        public async Task InitializeAsync(Guid userId)
        {
            if (_isInitialized) return;
            try
            {
                var response = await MessageApi.GetUserMessagesAsync(userId);

                if (response.IsSuccess)
                {
                    _conversations = response.Data;
                    _isInitialized = true;
                }
            }
            catch (Exception)
            {
                _isInitialized = false;
            }
        }

        public List<InboxViewModel> GetUserConversations()
        {
            return _conversations;
        }

        public void Clear()
        {
            _conversations.Clear();
            _isInitialized = false;
        }
    }
}
