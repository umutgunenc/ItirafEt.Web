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
        public ValueTask<T?> GetItemAsync<T>(string key)
        {
            var json = Preferences.Default.Get<string>(key,null);

            if (string.IsNullOrWhiteSpace(json))
                return ValueTask.FromResult<T?>(default);

            try
            {
                var result = JsonSerializer.Deserialize<T>(json);
                return ValueTask.FromResult<T?>(result);
            }
            catch
            {
                return ValueTask.FromResult<T?>(default);
            }
        }

        public ValueTask SetItemAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            Preferences.Default.Set(key, json);
            return ValueTask.CompletedTask;
        }

        public ValueTask RemoveItemAsync(string key)
        {
            Preferences.Default.Remove(key);
            return ValueTask.CompletedTask;
        }

        public ValueTask ClearItemsAsync()
        {
            Preferences.Default.Clear();
            return ValueTask.CompletedTask;
        }
    }
}
