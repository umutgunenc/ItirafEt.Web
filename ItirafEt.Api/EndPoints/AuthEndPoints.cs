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

            app.MapPost("/api/auth/forgotPassword", async (ForgotPaswordViewModel model, HttpContext context, AuthService authService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                model.IpAddress = ipAddress;
                model.DeviceInfo = userAgent;

                return Results.Ok(await authService.CreatePasswordResetTokenAsync(model));
            });

            app.MapPost("/api/auth/changeUserPassword", async (ChangeUserPasswordViewModel model, AuthService authService) =>
                Results.Ok(await authService.ChangeUserPasswordAsync(model)));
            return app;
        }
    }
}
