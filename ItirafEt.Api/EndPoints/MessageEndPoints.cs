using ItirafEt.Api.Services;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;

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
                Results.Ok(await messageService.GetConversationAsync(conversationId,senderUserId)))
                  .RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));


            app.MapPost("/api/message/SendMessage", async (Guid conversationId, MessageDto messageDto, HttpContext context, MessageService messageService) =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                messageDto.SenderIpAddress = ipAddress;
                messageDto.SenderDeviceInfo = userAgent;

                return Results.Ok(await messageService.SendMessageAsync(conversationId, messageDto));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            app.MapGet("/api/message/CanUserReadConversation", async (Guid conversationId, Guid userId, MessageService messageService) =>
            {
                var result = await messageService.CanUserReadConversationAsync(conversationId, userId);
                    return Results.Ok(result);

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.User)));

            return app;
        }

    }
}
