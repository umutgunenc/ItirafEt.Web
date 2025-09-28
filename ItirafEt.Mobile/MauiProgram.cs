using Blazored.LocalStorage;
using ItirafEt.Mobile.Services;
using ItirafEt.Shared.ClientServices;
using ItirafEt.Shared.Services;
using ItirafEt.Shared.Services.ClientServices.State;
using ItirafEt.SharedComponents.Apis;
using ItirafEt.SharedComponents.Auth;
using ItirafEt.SharedComponents.ClientServices;
using ItirafEt.SharedComponents.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Refit;
using ItirafEt.SharedComponents.Helpers;



#if ANDROID
using Xamarin.Android.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Android.Media.Midi;
#elif IOS
using Security;
#endif
namespace ItirafEt.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IStorageService, StorageService>();
            builder.Services.AddSingleton<IDateTimeHelperService, DateTimeHelperService>();

            builder.Services.AddBlazorBootstrap();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped(typeof(InfiniteScrollState<>));
            builder.Services.AddScoped<IScrollHelper, ScrollHelper>();
            builder.Services.AddScoped<SignalRInboxService>();
            builder.Services.AddScoped<IDeviceService, DeviceService>();


            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddSingleton<AuthStateProvider>();
            builder.Services.AddSingleton<InboxService>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
            builder.Services.AddAuthorizationCore();

            builder.Services.AddSingleton<IStorageService, StorageService>();
            builder.Services.AddScoped<ConfigureHubConnectionAsync, MobileSignalRService>();
            builder.Services.AddSingleton<IDateTimeHelperService, DateTimeHelperService>();

            ConfigureRefit(builder.Services);

            return builder.Build();
        }

        private static readonly string apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android 
                                                    ? ApiBaseUrl.AndroidBaseUrlHttps 
                                                    : ApiBaseUrl.BaseUrl;

        static void ConfigureRefit(IServiceCollection services)
        {

            //string baseUrl = ApiBaseUrl.BaseUrl;

            services.AddRefitClient<IAuthApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<ICategoryApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<IBanUserApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<IPostApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<ICommentApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<IReactionApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<IPostViewApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<IMessageApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<IUserSettingApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            services.AddRefitClient<IUserProfileApi>(GetRefitSettings)
                .ConfigureHttpClient(SetHttpClient);

            //static void SetHttpClient(HttpClient httpClient) => httpClient.BaseAddress = new Uri(baseUrl);
            void SetHttpClient(HttpClient httpClient) => httpClient.BaseAddress = new Uri(apiBaseUrl);

            static RefitSettings GetRefitSettings(IServiceProvider sp)
            {
                var authStateProvider = sp.GetRequiredService<AuthStateProvider>();
                return new RefitSettings
                {
                    AuthorizationHeaderValueGetter = (_, __) => Task.FromResult(authStateProvider.User?.Token ?? ""),

                    HttpMessageHandlerFactory = () =>
                    {
#if ANDROID
                        var androidMessageHandler = new Xamarin.Android.Net.AndroidMessageHandler();

                        androidMessageHandler.ServerCertificateCustomValidationCallback =
                        (HttpRequestMessage httpRequestMessage, X509Certificate2? certificate2, X509Chain? chain, SslPolicyErrors errors) =>
                            certificate2?.Issuer == "CN=localhost" || errors == SslPolicyErrors.None;

                        return androidMessageHandler;
#elif IOS 
                        var nsUrlSessionHandler = new NSUrlSessionHandler();
                        nsUrlSessionHandler.TrustOverrideForUrl = 
                        (NSUrlSessionHandler sender, string url, SecTrust trust) 
                            => url.StartsWith("http://localhost");
                        return nsUrlSessionHandler;

#endif

                        return null;
                       
                    }
                };
            }
        }
    }

}
