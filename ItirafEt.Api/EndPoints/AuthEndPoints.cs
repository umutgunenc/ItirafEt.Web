using ItirafEt.Api.Services;
using ItirafEt.Shared.DTOs;

namespace ItirafEt.Api.EndPoints
{
    public static class AuthEndPoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/auth/login", async (LoginDto dto, AuthService authService) =>
                Results.Ok(await authService.LoggingAsync(dto)));

            app.MapPost("/api/auth/register", async (RegisterDto dto, AuthService authService) =>
                Results.Ok(await authService.RegisterAsync(dto)));

            return app;
        }
    }
}
