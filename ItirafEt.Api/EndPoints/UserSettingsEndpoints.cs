using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ItirafEt.Api.EndPoints
{
    public static class UserSettingsEndpoints
    {
        public static IEndpointRouteBuilder MapUserSettingsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/getUserSettingsInfo", async (UserSettingService userSettingService, Guid userId) =>
               Results.Ok(await userSettingService.GetUserInfoAsync(userId)))
                   .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/changeUserSettingsInfo", async (UserSettingService userSettingService, UserSettingsInfoViewModel model, Guid userId) =>
               Results.Ok(await userSettingService.ChangeUserInfoAsync(userId, model)))
                   .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/changeUserPassword", async (UserSettingService userSettingService, UserSettingsChangePaswordViewModel model, Guid userId) =>
               Results.Ok(await userSettingService.ChangeUserPasswordAsync(userId, model)))
                   .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/userDeactive", async (UserSettingService userSettingService, UserDeactiveViewModel model, Guid userId) =>
                Results.Ok(await userSettingService.UserDeactiveAsync(userId, model)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/changeUserProfilePicture", async ([FromServices] UserSettingService userSettingService, [FromForm] ChangeProfilePictureModel model, [FromServices] IWebHostEnvironment env, HttpContext httpContext) =>
            {

                if (model.Photo != null)
                {
                    var ext = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!allowed.Contains(ext))
                        return Results.Ok(ApiResponses<MessageViewModel>.Fail("Geçersiz dosya uzantısı."));

                    if (model.Photo.Length > 10 * 1024 * 1024)
                        return Results.Ok(ApiResponses<MessageViewModel>.Fail("Fotoğraf boyutu 10 MB'dan büyük olamaz."));

                    //var uploadsFolder = Path.Combine(env.WebRootPath, "uploads", "ProfilePicture");
                    //Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var safePath = Path.Combine("PrivateFiles", "profilePicture", model.UserId.ToString());
                    var fullPath = Path.Combine(env.ContentRootPath, safePath, fileName);
                    Directory.CreateDirectory(safePath);
                    using var fs = new FileStream(fullPath, FileMode.Create);
                    await model.Photo.CopyToAsync(fs);
                    model.PhotoUrl = fileName;

                }

                var result = await userSettingService.ChangeUserProfilePictureAsync(model);
                return Results.Ok(result);

            })
            .Accepts<ChangeProfilePictureModel>("multipart/form-data")
            .Produces<ApiResponses<ChangeProfilePictureModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/deleteUserProfilePicture", async (UserSettingService userSettingService, Guid userId) =>
                Results.Ok(await userSettingService.DeleteProfilePictureAsync(userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            return app;
        }
    }
}
