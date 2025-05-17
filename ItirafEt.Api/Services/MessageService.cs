using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class MessageService
    {
        private readonly dbContext _context;
        private readonly List<string> _allowedRoles = new()
        {
            UserRoleEnum.SuperAdmin.ToString(),
            UserRoleEnum.Admin.ToString(),
            UserRoleEnum.Moderator.ToString(),
            UserRoleEnum.SuperUser.ToString()
        };
        public MessageService(dbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponses<ConversationDto>> GetConversationDtoAsync(Guid senderUserId, Guid receiverUserId)
        {
            var senderUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == senderUserId);

            if (senderUser == null)
                return ApiResponses<ConversationDto>.Fail("Gönderici Kullanıcı Bulunamadı.");
            var receiverUser = await _context.Users

                .FirstOrDefaultAsync(x => x.Id == receiverUserId);
            if (receiverUser == null)
                return ApiResponses<ConversationDto>.Fail("Alıcı Kullanıcı Bulunamadı.");

            if (senderUserId == receiverUserId)
                return ApiResponses<ConversationDto>.Fail("Kendinize mesaj gönderemezsiniz.");

            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    (c.InitiatorId == senderUserId && c.ResponderId == receiverUserId)
                    ||
                    (c.InitiatorId == receiverUserId && c.ResponderId == senderUserId)
                );

            if (conversation != null)
            {
                var conversationDto = ConversationToConversationDto(conversation);
                return ApiResponses<ConversationDto>.Success(conversationDto);
            }
            else
            {
                if (_allowedRoles.Contains(senderUser.RoleName))
                {
                    var newConversation = new Conversation
                    {
                        InitiatorId = senderUserId,
                        ResponderId = receiverUserId,
                    };

                    await _context.Conversations.AddAsync(newConversation);
                    _context.SaveChanges(); 

                    var conversationDto = ConversationToConversationDto(newConversation);
                    return ApiResponses<ConversationDto>.Success(conversationDto);
                }
                else
                    return ApiResponses<ConversationDto>.Fail("Mesaj gönderebilmek için super kullanıcı olmalısınız.");
            }
        }

        public async Task<ApiResponses<MessageDto>> SendMessageAsync(Guid conversationId, MessageDto messageDto)
        {
            var senderUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == messageDto.SenderId);

            if (senderUser == null)
                return ApiResponses<MessageDto>.Fail("Gönderici Kullanıcı Bulunamadı.");

            var receiverUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == messageDto.ReceiverId);
            if (receiverUser == null)
                return ApiResponses<MessageDto>.Fail("Alıcı Kullanıcı Bulunamadı.");

            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation == null)
                return ApiResponses<MessageDto>.Fail("Mesaj gönderilemedi.");

            if (!((conversation.InitiatorId == messageDto.SenderId && conversation.ResponderId == messageDto.ReceiverId) || (conversation.InitiatorId == messageDto.ReceiverId && conversation.ResponderId == messageDto.SenderId)))
                return ApiResponses<MessageDto>.Fail("Mesaj gönderilemedi.");

            var message = new Message
            {
                Content = messageDto.Content,
                ConversationId = conversationId,
                SenderId = messageDto.SenderId,
                SentDate = DateTime.Now,
                IsRead = false,
                IsVisibleToInitiatorUser = true,
                IsVisibleToResponderUser = true,
                ReadDate = null,
                IpAddress = messageDto.SenderIpAddress,
                DeviceInfo = messageDto.SenderDeviceInfo

            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();


            var returnMessageDto = new MessageDto
            {
                SenderUserName = senderUser.UserName,
                SenderProfileImageUrl = senderUser.ProfilePictureUrl,
                ReceiverUserName = receiverUser.UserName,
                ReceiverProfileImageUrl = receiverUser.ProfilePictureUrl,
                Content = message.Content,
                CreatedDate = message.SentDate,
                SenderId = message.SenderId,

            };
            return ApiResponses<MessageDto>.Success(returnMessageDto);

        }


        private ConversationDto ConversationToConversationDto(Conversation conversation)
        {
            return new ConversationDto
            {
                ConversationId = conversation.ConversationId,
                InitiatorId = conversation.InitiatorId,
                ResponderId = conversation.ResponderId,
            };
        }

    }
}
