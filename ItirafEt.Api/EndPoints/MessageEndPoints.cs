using System.Linq;
using ItirafEt.Api.Services;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace ItirafEt.Api.EndPoints
{
    public static class MessageEndPoints
    {
        public static IEndpointRouteBuilder MapMessageEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/message/GetConversationDto", async (Guid senderUserId, Guid receiverUserId, MessageService messageService) =>
                  Results.Ok(await messageService.GetConversationDtoAsync(senderUserId, receiverUserId)))
                    .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/message/GetConversation", async (Guid conversationId, Guid senderUserId, MessageService messageService) =>
                Results.Ok(await messageService.GetConversationAsync(conversationId, senderUserId)))
                  .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/message/SendMessage", async (CreateMessageDto messageDto, HttpContext context, MessageService messageService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                messageDto.SenderIpAddress = ipAddress;
                messageDto.SenderDeviceInfo = userAgent;

                return Results.Ok(await messageService.SendMessageAsync(messageDto));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            //app.MapPost("/api/message/SendMessageWithPhoto", async (HttpContext context, IWebHostEnvironment env, [FromForm] string ConversationId, [FromForm] string Content, [FromForm] string SenderId, [FromForm] string ReceiverId, [FromForm] IFormFile? Photo, MessageService messageService) =>
            //{
            //    var messageDto = new CreateMessageDto
            //    {
            //        ConversationId = ConversationId,
            //        Content = Content,
            //        SenderId = SenderId,
            //        ReceiverId = ReceiverId,
            //        CreatedDate = DateTime.UtcNow,
            //        SenderIpAddress = context.Connection.RemoteIpAddress?.ToString(),
            //        SenderDeviceInfo = context.Request.Headers["User-Agent"].ToString(),
            //        Photo = Photo
            //    };

            //    if (Photo != null)
            //    {
            //        var ext = Path.GetExtension(Photo.FileName).ToLowerInvariant();
            //        if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(ext))
            //            return Results.BadRequest("Geçersiz dosya uzantısı.");

            //        if (Photo.Length > 10 * 1024 * 1024)
            //            return Results.BadRequest("Fotoğraf boyutu 10 MB'dan büyük olamaz.");

            //        var uploads = Path.Combine(env.WebRootPath, "uploads", "messages");
            //        Directory.CreateDirectory(uploads);

            //        var fileName = $"{Guid.NewGuid()}{ext}";
            //        var path = Path.Combine(uploads, fileName);
            //        await using var fileStream = File.Create(path);
            //        await Photo.CopyToAsync(fileStream);

            //        messageDto.PhotoUrl = $"/uploads/messages/{fileName}";
            //    }

            //    return Results.Ok(await messageService.SendMessageWithPhotoAsync(messageDto));

            //}).Accepts<MessageDto>("multipart/form-data")
            //  .Produces<ApiResponses<MessageDto>>(StatusCodes.Status200OK)
            //  .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)))
            //  .DisableAntiforgery();

            app.MapPost("/api/message/SendMessageWithPhoto", async ([FromServices] MessageService messageService, [FromForm] CreateMessageDto dto, [FromServices] IWebHostEnvironment env, HttpContext httpContext) =>
            {

                dto.SenderIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
                dto.SenderDeviceInfo = httpContext.Request.Headers["User-Agent"];


                if (dto.Photo != null)
                {
                    var ext = Path.GetExtension(dto.Photo.FileName).ToLowerInvariant();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!allowed.Contains(ext))
                        return Results.Ok(ApiResponses<MessageDto>.Fail("Geçersiz dosya uzantısı."));


                    if (dto.Photo.Length > 10 * 1024 * 1024)
                        return Results.Ok(ApiResponses<MessageDto>.Fail("Fotoğraf boyutu 10 MB'dan büyük olamaz."));

                    var uploadsFolder = Path.Combine(env.WebRootPath, "uploads", "messages");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var safePath = Path.Combine("PrivateFiles", "messages",dto.ConversationId);
                    var fullPath = Path.Combine(env.ContentRootPath, safePath, fileName);
                    Directory.CreateDirectory(safePath);
                    using var fs = new FileStream(fullPath, FileMode.Create);
                    await dto.Photo.CopyToAsync(fs);
                    //dto.PhotoUrl = $"/{safePath}/{fileName}";
                    dto.PhotoUrl = fileName;


                    //var filePath = Path.Combine(uploadsFolder, fileName);
                    //using var fs = new FileStream(filePath, FileMode.Create);
                    //await dto.Photo.CopyToAsync(fs);

                    //dto.PhotoUrl = $"/uploads/messages/{fileName}";
                }

                var result = await messageService.SendMessageWithPhotoAsync(dto);
                return Results.Ok(result);

            })
            .Accepts<CreateMessageDto>("multipart/form-data")
            .Produces<ApiResponses<MessageDto>>(StatusCodes.Status200OK)
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

            app.MapGet("/api/message/GetUserConversaions", async (Guid userId, MessageService messageService) =>
            {
                var result = await messageService.GetUserConversaionsAsync(userId);
                return Results.Ok(result);

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/message/readMessage", async (Guid conversationId, MessageDto messageDto, MessageService messageService) =>
            {
                return Results.Ok(await messageService.ReadMessageAsync(conversationId, messageDto));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapPost("/api/message/getConversationMessages", async (ConversationDto conversation, DateTime? nextBefore, int take, MessageService messageService) =>
                Results.Ok(await messageService.GetConversationMessagesAsync(conversation, nextBefore, take)))
                .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));



            return app;
        }

    }
}
