using Blazored.LocalStorage;
using ItirafEt.Shared.ClientServices;
using ItirafEt.Shared.Services;
using ItirafEt.Shared.Services.ClientServices.State;
using ItirafEt.SharedComponents.Apis;
using ItirafEt.SharedComponents.Auth;
using ItirafEt.SharedComponents.ClientServices;
using ItirafEt.SharedComponents.HelperStrings;
using ItirafEt.SharedComponents.Services;
using ItirafEt.Web;
using ItirafEt.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorBootstrap();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(typeof(InfiniteScrollState<>));
builder.Services.AddScoped<IScrollHelper, ScrollHelper>();
builder.Services.AddScoped<SignalRInboxService>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthStateProvider>();
builder.Services.AddSingleton<InboxService>();
builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
builder.Services.AddAuthorizationCore();

builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddScoped<ISignalRService, WebSignalRService>();




ConfigureRefit(builder.Services);

await builder.Build().RunAsync();

static void ConfigureRefit(IServiceCollection services)
{
    //const string baseUrl = "https://localhost:7292";
    string baseUrl = ApiBaseUrl.BaseUrl;

    services.AddRefitClient<IAuthApi>()
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
    void SetHttpClient(HttpClient httpClient) => httpClient.BaseAddress = new Uri(baseUrl);

    static RefitSettings GetRefitSettings(IServiceProvider sp)
    {
        var authStateProvider = sp.GetRequiredService<AuthStateProvider>();
        return new RefitSettings
        {
            AuthorizationHeaderValueGetter = (_, __) => Task.FromResult(authStateProvider.User?.Token ?? "")
        };
    }
}
