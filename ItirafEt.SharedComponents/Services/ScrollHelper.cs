using ItirafEt.Shared.ClientServices;
using Microsoft.JSInterop;

namespace ItirafEt.SharedComponents.Services
{
    public class ScrollHelper : IScrollHelper
    {
        private readonly IJSRuntime _javaScript;

        public ScrollHelper(IJSRuntime javaScript)
        {
            _javaScript = javaScript;
        }

        public ValueTask SetScrollYAsync(double y) => _javaScript.InvokeVoidAsync("scrollHelper.setScrollY", y);
    }
}
