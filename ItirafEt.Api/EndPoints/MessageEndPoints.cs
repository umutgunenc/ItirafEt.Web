using System;
using System.Linq;
using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.Services.ClientServices.State;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Refit;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ItirafEt.Api.EndPoints
{
    public static class MessageEndPoints
    {
        public static IEndpointRouteBuilder MapMessageEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/message/GetConversationDto", async (Guid senderUserId, Guid receiverUserId, MessageService messageService) =>
                  Results
                  .Ok(await messageService.GetConversationDtoAsync(senderUserId, receiverUserId)))
                  .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/message/GetConversation", async (Guid conversationId, Guid senderUserId, MessageService messageService) =>
                Results
                .Ok(await messageService.GetConversationAsync(conversationId, senderUserId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));



            app.MapPost("/api/message/SendMessage", async (CreateMessageViewModel model, HttpContext context, MessageService messageService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                model.SenderIpAddress = ipAddress;
                model.SenderDeviceInfo = userAgent;

                return Results.Ok(await messageService.SendMessageAsync(model));


            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/message/SendMessageWithPhoto", async ([FromServices] MessageService messageService, [FromForm] CreateMessageViewModel model, [FromServices] IWebHostEnvironment env, HttpContext httpContext) =>
            {

                model.SenderIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
                model.SenderDeviceInfo = httpContext.Request.Headers["User-Agent"];


                if (model.Photo != null)
                {
                    var ext = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!allowed.Contains(ext))
                        return Results.Ok(ApiResponses<MessageViewModel>.Fail("Geçersiz dosya uzantısı."));


                    if (model.Photo.Length > 10 * 1024 * 1024)
                        return Results.Ok(ApiResponses<MessageViewModel>.Fail("Fotoğraf boyutu 10 MB'dan büyük olamaz."));


                    var photoFileName = $"{Guid.NewGuid()}{ext}";
                    var photoSafePath = Path.Combine("PrivateFiles", "messages", model.ConversationId, "Photo");
                    var photoFullPath = Path.Combine(env.ContentRootPath, photoSafePath, photoFileName);
                    Directory.CreateDirectory(Path.Combine(env.ContentRootPath, photoSafePath));
                    using var photoImage = await Image.LoadAsync(model.Photo.OpenReadStream());

                    var encoder = new JpegEncoder()
                    {
                        Quality = 75
                    };

                    await photoImage.SaveAsync(photoFullPath, encoder);

                    var thumbnailFileName = $"{Path.GetFileNameWithoutExtension(photoFileName)}_thumb{ext}";
                    var thumbnailSafePath = Path.Combine("PrivateFiles", "messages", model.ConversationId, "Thumbnail");
                    var thumbnailFullPath = Path.Combine(env.ContentRootPath, thumbnailSafePath, thumbnailFileName);
                    Directory.CreateDirectory(Path.Combine(env.ContentRootPath, thumbnailSafePath));
                    using (var thumbnailImage = Image.Load(model.Photo.OpenReadStream()))
                    {
                        thumbnailImage.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(150, 150),
                            Mode = ResizeMode.Max
                        }));
                        await thumbnailImage.SaveAsync(thumbnailFullPath);
                    }

                    model.PhotoId = photoFileName;
                    model.ThumbnailId = thumbnailFileName;
                }

                var result = await messageService.SendMessageWithPhotoAsync(model);
                return Results.Ok(result);


            })
            .Accepts<CreateMessageViewModel>("multipart/form-data")
            .Produces<ApiResponses<MessageViewModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();



            app.MapGet("/api/message/photo", (string token, MessageService messageService) =>
            {
                var result = messageService.GetMessagePhoto(token);
                return result;
            });

            app.MapGet("/api/message/thumbnail", (string token, MessageService messageService) =>
            {
                return messageService.GetMessagePhotoThumbnail(token);
            });

            app.MapGet("/api/message/getSignedPhotoUrl", async (string photoId, Guid userId, MessageService messageService) =>
                Results
                .Ok(await messageService.GetSignedPhotoUrlAsync(photoId, userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapGet("/api/message/CanUserReadConversation", async (Guid conversationId, Guid userId, MessageService messageService) =>
                Results
                .Ok(await messageService.CanUserReadConversationAsync(conversationId, userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));



            app.MapPost("/api/message/readMessage", async (Guid conversationId, MessageViewModel messageDto, MessageService messageService) =>
                Results
                .Ok(await messageService.ReadMessageAsync(conversationId, messageDto)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/message/getConversationMessages", async (GetConversationMessageViewModel model, MessageService messageService) =>
                 Results
                 .Ok(await messageService.GetConversationMessagesAsync(model)))
                 .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapGet("/api/message/GetUserMessages", async (Guid userId, MessageService messageService) =>
                Results
                .Ok(await messageService.GetUserMessagesAsync(userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapGet("/api/message/CheckUnreadMessages", async (Guid userId, MessageService messageService) =>
                Results
                .Ok(await messageService.CheckUnreadMessagesAsync(userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            return app;
        }
    }
}
