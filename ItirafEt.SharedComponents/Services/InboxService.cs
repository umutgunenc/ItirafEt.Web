
using ItirafEt.Shared.ViewModels;
using ItirafEt.SharedComponents.Apis;

namespace ItirafEt.SharedComponents.ClientServices
{
    public class InboxService :IInboxService
    {
        private Task? _initializationTask;
        private List<InboxViewModel> _conversations = new();
        public bool _isInitialized = false;
        public string? _errorMessage { get; set; }
        public bool _haveError { get; set; }

        private readonly IMessageApi MessageApi;

        public InboxService(IMessageApi messageApi)
        {
            MessageApi = messageApi;
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
        //private async Task InitializeInternalAsync(Guid userId)
        //{
        //    try
        //    {
        //        var response = await MessageApi.GetUserMessagesAsync(userId);
        //        if (response.IsSuccess)
        //        {
        //            _conversations = response.Data;
        //            _haveError = false;
        //            _isInitialized = true;
        //        }
        //        else
        //        {
        //            _errorMessage = response.ErrorMessage;
        //            _haveError = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorMessage = ex.Message;
        //        _haveError = true;
        //    }
        //}

        private async Task InitializeInternalAsync(Guid userId)
        {
            try
            {
                var response = await MessageApi.GetUserMessagesAsync(userId);
                if (response.IsSuccess)
                {
                    _conversations.Clear();                        // ✅ referansı koruyor
                    _conversations.AddRange(response.Data);        // yeni verileri ekle
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

        public ref List<InboxViewModel> GetUserConversations()
        {
            return ref _conversations;
        }

        public void Clear()
        {
            _conversations.Clear();
            _isInitialized = false;
            _haveError = false;
            _errorMessage = null;
            _initializationTask = null;
        }

    }
}
