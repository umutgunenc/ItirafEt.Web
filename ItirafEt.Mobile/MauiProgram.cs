using Blazored.LocalStorage;
using ItirafEt.Mobile.Services;
using ItirafEt.Shared.ClientServices;
using ItirafEt.Shared.Services;
using ItirafEt.Shared.Services.ClientServices.State;
using ItirafEt.SharedComponents.Auth;
using ItirafEt.SharedComponents.ClientServices;
using ItirafEt.SharedComponents.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

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

            builder.Services.AddBlazorBootstrap();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped(typeof(InfiniteScrollState<>));
            builder.Services.AddScoped<IScrollHelper, ScrollHelper>();
            builder.Services.AddScoped<SignalRInboxService>();
            //builder.Services.AddSingleton<ApiBaseUrl>();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddSingleton<AuthStateProvider>();
            builder.Services.AddSingleton<InboxService>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
            builder.Services.AddAuthorizationCore();

            return builder.Build();
        }
    }
}
