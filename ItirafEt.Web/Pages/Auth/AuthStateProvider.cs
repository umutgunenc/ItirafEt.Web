using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using ItirafEt.Shared;
using Microsoft.AspNetCore.Components;
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
        private Timer _tokenCheckTimer;


        public AuthStateProvider(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
            SetAuthStateTask();

        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authStateTask;


        public LoggedInUser? User { get; private set; }
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
            _tokenCheckTimer?.Dispose();

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

                if (!IsTokenValid(user.Token))
                {
                    await SetLogoutAsync();
                    return;
                }


                await SetLoginAsync(user);
                await StartTokenMonitorAsync();
            }
            finally
            {
                IsInitializing = false;
                // TODO araştır gerek var mi 
                //NotifyAuthenticationStateChanged(_authStateTask);
            }
        }

        private static bool IsTokenValid(string Token)
        {
            if (string.IsNullOrWhiteSpace(Token))
                return false;

            var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(Token))
                return false;

            var jwt = jwtHandler.ReadJwtToken(Token);
            jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (string.IsNullOrWhiteSpace(expClaim))
                return false;

            var expTime = long.Parse(expClaim);

            var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expTime).UtcDateTime;

            return expDateTime > DateTime.UtcNow;

        }
        private void SetAuthStateTask()
        {
            var identity = new ClaimsIdentity();

            if (IsLoggedIn)
                identity = new ClaimsIdentity(User.ToClaims(), AuthType);

            var principal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);
            _authStateTask = Task.FromResult(authState);

        }

        private async Task StartTokenMonitorAsync()
        {
            _tokenCheckTimer = new Timer(async _ =>
            {
                if (User != null && !IsTokenValid(User.Token))
                    await SetLogoutAsync();

            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        }


        //private void StartTokenMonitor()
        //{
        //    _tokenCheckTimer = new Timer(_ =>
        //    {
        //        _ = CheckTokenAndLogoutAsync(); // Fire-and-forget async task

        //    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        //}

        //private async Task CheckTokenAndLogoutAsync()
        //{
        //    if (User != null && !IsTokenValid(User.Token))
        //        await SetLogoutAsync();
        //}
    }
}
