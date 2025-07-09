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

            app.MapPost("/api/changeProfileVisibilty", async (UserSettingService userSettingService, Guid userId) =>
                Results.Ok(await userSettingService.ChangeProfileVisibilty(userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof    (UserRoleEnum.SuperUser),nameof(UserRoleEnum.User)));

            app.MapPost("/api/changeUserProfilePicture", async ([FromServices] UserSettingService userSettingService, [FromForm] ChangeProfilePictureModel model, [FromServices] IWebHostEnvironment env, HttpContext httpContext, HttpRequest req) =>
            {

                if (model.Photo != null)
                {
                    var ext = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!allowed.Contains(ext))
                        return Results.Ok(ApiResponses<MessageViewModel>.Fail("Geçersiz dosya uzantısı."));

                    if (model.Photo.Length > 10 * 1024 * 1024)
                        return Results.Ok(ApiResponses<MessageViewModel>.Fail("Fotoğraf boyutu 10 MB'dan büyük olamaz."));

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var uploadFolder = Path.Combine(env.WebRootPath, "profilepicture", model.UserId.ToString());
                    Directory.CreateDirectory(uploadFolder);
                    var fullPath = Path.Combine(uploadFolder, fileName);

                    await using var fs = new FileStream(fullPath, FileMode.Create);
                    await model.Photo.CopyToAsync(fs);

                    var relativeUrl = Path.Combine("profilepicture", model.UserId.ToString(), fileName)
                       .Replace(Path.DirectorySeparatorChar, '/');


                    var absoluteUrl = $"{req.Scheme}://{req.Host.Value.TrimEnd('/')}/{relativeUrl}";
                    model.PhotoUrl = absoluteUrl;

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
