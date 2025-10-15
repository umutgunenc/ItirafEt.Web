using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.Services
{
    public interface IStorageService
    {
        ValueTask<T?> GetItemAsync<T>(string key, bool isSecure);
        ValueTask SetItemAsync<T>(string key, T value, bool isSecure);
        ValueTask RemoveItemAsync(string key, bool isSecure);
        ValueTask ClearItemsAsync();
    }
}
