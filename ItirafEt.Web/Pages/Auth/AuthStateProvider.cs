using System.Security.Claims;
using System.Text.Json;
using ItirafEt.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ItirafEt.Web.Pages.Auth
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private const string AuthType = "ItirafEt";
        private const string UserDataKey = "uData";
        private Task<AuthenticationState> _authStateTask;
        private readonly IJSRuntime _jSRuntime;

        public AuthStateProvider(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
            SetAuthStateTask();

        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authStateTask;

        public LoggedInUser User { get; private set; }
        public bool IsInitializing { get; private set; } = true;
        public bool IsLoggedIn => User != null;

        public async Task SetLoginAsync(LoggedInUser user)
        {
            User = user;
            SetAuthStateTask();
            NotifyAuthenticationStateChanged(_authStateTask);
            await _jSRuntime.InvokeVoidAsync("localStorage.setItem", UserDataKey, user.ToJson());

        }

        public async Task SetLogoutAsync()
        {
            User = null;
            SetAuthStateTask();
            NotifyAuthenticationStateChanged(_authStateTask);
            await _jSRuntime.InvokeVoidAsync("localStorage.removeItem", UserDataKey);

        }

        public async Task InitializeAsync()
        {
            try
            {
                var uData = await _jSRuntime.InvokeAsync<string?>("localStorage.getItem", UserDataKey);
                if (string.IsNullOrWhiteSpace(uData))
                    return;

                var user = LoggedInUser.FromJson(uData);

                if (user == null)
                    return;

                await SetLoginAsync(user);
            }
            finally
            {
                IsInitializing = false;
                // TODO araştır gerek var mi 
                //NotifyAuthenticationStateChanged(_authStateTask);
            }
        }
        private void SetAuthStateTask()
        {
            if (IsLoggedIn)
            {
                var identity = new ClaimsIdentity(User.ToClaims(), AuthType);
                var principal = new ClaimsPrincipal(identity);
                var authState = new AuthenticationState(principal);
                _authStateTask = Task.FromResult(authState);
            }
            else
            {
                var identity = new ClaimsIdentity();
                var principal = new ClaimsPrincipal(identity);
                var authState = new AuthenticationState(principal);
                _authStateTask = Task.FromResult(authState);
            }

        }
    }
}
