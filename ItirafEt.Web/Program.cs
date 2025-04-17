using ItirafEt.Web;
using ItirafEt.Web.Apis;
using ItirafEt.Web.Pages.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
builder.Services.AddAuthorizationCore();

ConfigureRefit(builder.Services);

await builder.Build().RunAsync();


static void ConfigureRefit(IServiceCollection services)
{
    const string baseUrl = "https://localhost:7292";

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

    static void SetHttpClient(HttpClient httpClient) => httpClient.BaseAddress = new Uri(baseUrl);

    static RefitSettings GetRefitSettings(IServiceProvider sp)
    {
        var authStateProvider = sp.GetRequiredService<AuthStateProvider>();
        return new RefitSettings
        {
            AuthorizationHeaderValueGetter = (_, __) => Task.FromResult(authStateProvider.User?.Token??"")
        };
    }
}
