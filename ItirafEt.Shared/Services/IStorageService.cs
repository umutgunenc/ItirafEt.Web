using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.Services
{
    public interface IStorageService
    {
        ValueTask<T?> GetItemAsync<T>(string key);
        ValueTask SetItemAsync<T>(string key, T value);
        ValueTask RemoveItemAsync(string key);
        ValueTask ClearItemsAsync();
    }
}
