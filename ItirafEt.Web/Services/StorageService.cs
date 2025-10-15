using System.Text.Json;
using ItirafEt.Shared.Services;
using Microsoft.JSInterop;

namespace ItirafEt.Web.Services
{
    public class StorageService : IStorageService
    {
        private readonly IJSRuntime _jsRuntime;
        public StorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }


        public async ValueTask<T?> GetItemAsync<T>(string key, bool isSecure = false)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
        public async ValueTask RemoveItemAsync(string key, bool isSecure = false) => await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        public async ValueTask SetItemAsync<T>(string key, T value, bool isSecure = false)
        {
            var json = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        public async ValueTask ClearItemsAsync() => await _jsRuntime.InvokeVoidAsync("localStorage.clear");

    }
}
