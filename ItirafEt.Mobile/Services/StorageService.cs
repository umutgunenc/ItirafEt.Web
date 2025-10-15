using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ItirafEt.Shared.Services;

namespace ItirafEt.Mobile.Services
{
    public class StorageService : IStorageService
    {
        public async ValueTask<T?> GetItemAsync<T>(string key, bool isSecure = false)
        {
            string? json = null;

            if (isSecure)
                json = await SecureStorage.Default.GetAsync(key);
            else
                json = Preferences.Default.Get<string>(key, null);


            if (string.IsNullOrWhiteSpace(json))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }

        public async ValueTask SetItemAsync<T>(string key, T value, bool isSecure = false)
        {
            var json = JsonSerializer.Serialize(value);

            if (isSecure)
                await SecureStorage.Default.SetAsync(key, json);
            else
                Preferences.Default.Set(key, json);

        }

        public ValueTask RemoveItemAsync(string key, bool isSecure = false)
        {

            if (isSecure)
                SecureStorage.Default.Remove(key);
            else
                Preferences.Default.Remove(key);

            return ValueTask.CompletedTask;

        }

        public ValueTask ClearItemsAsync()
        {
            SecureStorage.Default.RemoveAll();
            Preferences.Default.Clear();
            return ValueTask.CompletedTask;
        }
    }
}
