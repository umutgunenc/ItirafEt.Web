using ItirafEt.SharedComponents.Services;
using Microsoft.JSInterop;

namespace ItirafEt.Web.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IJSRuntime _jsRuntime;
        private string? _userAgent;
        private bool _initialized;

        public DeviceService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public string Platform => _userAgent ?? "Web";


        public async Task<bool> IsMobileAsync()
        {
            if (!_initialized)
                await InitializeAsync();

            if (_userAgent?.ToLower().Contains("mobile")==true)
                return true;
            return false;

        }

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;
            _userAgent = await _jsRuntime.InvokeAsync<string>("eval", "navigator.userAgent");
            _initialized = true;
        }
    }
}
