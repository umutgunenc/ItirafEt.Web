using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using ItirafEt.Shared;
using ItirafEt.Shared.Services;
using ItirafEt.SharedComponents.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ItirafEt.SharedComponents.Auth
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private const string AuthType = "ItirafEt";
        private const string UserDataKey = "uData";
        private Task<AuthenticationState> _authStateTask;
        private readonly IStorageService _storageService;
        private Timer _tokenCheckTimer;
        private readonly ISignalRService _signalRService;


        public AuthStateProvider(IJSRuntime jSRuntime, IStorageService storageService, ISignalRService signalRService)
        {
            _storageService = storageService;
            SetAuthStateTask();
            _signalRService = signalRService;
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
            await _storageService.SetItemAsync(UserDataKey, user.ToJson());

        }
        public async Task SetLogoutAsync()
        {
            User = null;
            SetAuthStateTask();
            NotifyAuthenticationStateChanged(_authStateTask);
            await _signalRService.DisposeAllAsync();
            await _storageService.ClearItemsAsync();
            _tokenCheckTimer?.Dispose();

        }

        public async Task InitializeAsync()
        {
            try
            {
                var uData = await _storageService.GetItemAsync<string>(UserDataKey);

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
            //var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

            //if (string.IsNullOrWhiteSpace(expClaim))
            //    return false;

            if (expClaim == null)
                return false;

            if (!long.TryParse(expClaim.Value, out var expTime))
                return false;

            var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expTime).UtcDateTime;
            return expDateTime > DateTime.UtcNow;

            //var expTime = long.Parse(expClaim);

            //var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expTime).UtcDateTime;

            //return expDateTime > DateTime.UtcNow;

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


        public async Task UpdateUserNameAsync(string newUserName)
        {
            if (User is null)
                return;

            var updatedUser = User with { userName = newUserName };

            User = updatedUser;

            await _storageService.SetItemAsync(UserDataKey, updatedUser.ToJson());

            var identity = new ClaimsIdentity(updatedUser.ToClaims(), AuthType);
            var principal = new ClaimsPrincipal(identity);
            _authStateTask = Task.FromResult(new AuthenticationState(principal));
            NotifyAuthenticationStateChanged(_authStateTask);
        }
    }
}
