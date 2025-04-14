using ItirafEt.Api.Services;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.EndPoints
{
    public static class BanUserEndPoints
    {
        public static IEndpointRouteBuilder MapBanUserEndPoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/api/getAllUsers", async (BanUserService banUserService) =>
                Results.Ok(await banUserService.GetAllUsers()))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRole.SuperAdmin), nameof(UserRole.Admin),nameof(UserRole.Moderator)));

            app.MapPost("/api/banUser", async (BanUserDto bannedUser,Guid adminId, BanUserService banUserService) =>
                Results.Ok(await banUserService.BanUser(bannedUser, adminId)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRole.SuperAdmin), nameof(UserRole.Admin), nameof(UserRole.Moderator)));

            return app;
        }
    }
}
