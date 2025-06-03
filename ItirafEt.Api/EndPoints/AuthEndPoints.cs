using ItirafEt.Api.Services;
using ItirafEt.Shared.ViewModels;

namespace ItirafEt.Api.EndPoints
{
    public static class AuthEndPoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/auth/login", async (LoginViewModel model, AuthService authService) =>
                Results.Ok(await authService.LoggingAsync(model)));

            app.MapPost("/api/auth/register", async (RegisterViewModel model, AuthService authService) =>
                Results.Ok(await authService.RegisterAsync(model)));

            return app;
        }
    }
}
