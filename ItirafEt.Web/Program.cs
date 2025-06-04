using Blazored.LocalStorage;
using ItirafEt.Shared.ClientServices;
using ItirafEt.Shared.ClientServices.State;
using ItirafEt.SharedComponents.Apis;
using ItirafEt.SharedComponents.ClientServices;
using ItirafEt.SharedComponents.Services;
using ItirafEt.Web;
using ItirafEt.Web.Pages.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazorBootstrap();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(typeof(InfiniteScrollState<>));
builder.Services.AddScoped<IScrollHelper, ScrollHelper>();
//builder.Services.AddSingleton<ApiBaseUrl>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthStateProvider>();
builder.Services.AddSingleton<InboxService>();
builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
builder.Services.AddAuthorizationCore();

// dinmaik olarak API adresini alabilmek için
//builder.Services.Configure<ApiBaseUrl>(builder.Configuration.GetSection("ApiSettings"));


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
