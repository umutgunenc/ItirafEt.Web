using System.Linq;
using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Refit;

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

                    //var uploadsFolder = Path.Combine(env.WebRootPath, "uploads", "messages");
                    //Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var safePath = Path.Combine("PrivateFiles", "messages", model.ConversationId);
                    var fullPath = Path.Combine(env.ContentRootPath, safePath, fileName);
                    Directory.CreateDirectory(safePath);
                    using var fs = new FileStream(fullPath, FileMode.Create);
                    await model.Photo.CopyToAsync(fs);
                    //dto.PhotoUrl = $"/{safePath}/{fileName}";
                    model.PhotoUrl = fileName;


                    //var filePath = Path.Combine(uploadsFolder, fileName);
                    //using var fs = new FileStream(filePath, FileMode.Create);
                    //await dto.Photo.CopyToAsync(fs);

                    //dto.PhotoUrl = $"/uploads/messages/{fileName}";
                }

                var result = await messageService.SendMessageWithPhotoAsync(model);
                return Results.Ok(result);

            })
            .Accepts<CreateMessageViewModel>("multipart/form-data")
            .Produces<ApiResponses<MessageViewModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();


            app.MapGet("/api/message/photo", async (string filename, HttpContext http, MessageService messageService) =>
            {
                return await messageService.GetMessagePhotoAsync(filename, http.User);

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapGet("/api/message/CanUserReadConversation", async (Guid conversationId, Guid userId, MessageService messageService) =>
            {
                var result = await messageService.CanUserReadConversationAsync(conversationId, userId);
                return Results.Ok(result);

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            //app.MapGet("/api/message/GetUserConversaions", async (Guid userId, MessageService messageService) =>
            //{
            //    var result = await messageService.GetUserConversaionsAsync(userId);
            //    return Results.Ok(result);

            //}).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/message/readMessage", async (Guid conversationId, MessageViewModel messageDto, MessageService messageService) =>
            {
                return Results.Ok(await messageService.ReadMessageAsync(conversationId, messageDto));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/message/getConversationMessages", async (ConversationViewModel conversation, DateTime? nextBefore, int take, MessageService messageService) =>
                Results
                .Ok(await messageService.GetConversationMessagesAsync(conversation, nextBefore, take)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapGet("/api/message/GetUserMessages", async (Guid userId, MessageService messageService) =>
                Results
                .Ok(await messageService.GetUserMessagesAsync(userId)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapGet("/api/message/CheckUnreadMessages", async (Guid userId, MessageService messageService) =>
            {
                var result = await messageService.CheckUnreadMessagesAsync(userId);
                return Results.Ok(result);
            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            return app;
        }
    }
}
