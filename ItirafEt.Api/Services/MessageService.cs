using System.Net.NetworkInformation;
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
        private readonly MessageHubService _hubService;
        private readonly List<string> _allowedRoles = new()
        {
            UserRoleEnum.SuperAdmin.ToString(),
            UserRoleEnum.Admin.ToString(),
            UserRoleEnum.Moderator.ToString(),
            UserRoleEnum.SuperUser.ToString()
        };
        public MessageService(dbContext context, MessageHubService hubService)
        {
            _context = context;
            _hubService = hubService;
        }

        public async Task<ApiResponses<List<ConversationDto>>> GetUserConversaionsAsync(Guid userId)
        {
            var conversations = await _context.Conversations
                .AsNoTracking()
                .Where(c => c.InitiatorId == userId || c.ResponderId == userId)
                .ToListAsync();

            if (conversations.Count == 0)
                return ApiResponses<List<ConversationDto>>.Fail("Mesajlaşma Bulunamadı.");

            var returnConversationsDto  = new List<ConversationDto>();
            foreach (var conversation in conversations)
            {
                var responderUserId = conversation.InitiatorId == userId ? conversation.ResponderId : conversation.InitiatorId;
                var conversationDto = await ConversationToConversationDtoWithLastMessageAsync(conversation, responderUserId, userId);
                returnConversationsDto.Add(conversationDto);
            }


            return ApiResponses<List<ConversationDto>>.Success(returnConversationsDto);
        }
        public async Task<ApiResponses<bool>> CanUserReadConversationAsync(Guid conversationId, Guid userId)
        {
            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation == null)
                return ApiResponses<bool>.Fail("Mesajlaşma Bulunamadı.");

            if (conversation.InitiatorId != userId && conversation.ResponderId != userId)
                return ApiResponses<bool>.Fail("Mesajlaşma Bulunamadı.");

            return ApiResponses<bool>.Success(true);
        }
        public async Task<ApiResponses<ConversationDto>> GetConversationAsync(Guid conversationId, Guid senderUserId)
        {
            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation == null)
                return ApiResponses<ConversationDto>.Fail("Mesajlaşma Bulunamadı");
            var receiverUserId = conversation.InitiatorId == senderUserId ? conversation.ResponderId : conversation.InitiatorId;
            var conversationDto = await ConversationToConversationDtoAsync(conversation, receiverUserId, senderUserId);

            return ApiResponses<ConversationDto>.Success(conversationDto);

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
                var conversationDto = await ConversationToConversationDtoAsync(conversation, receiverUserId, senderUserId);
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

                    var conversationDto = await ConversationToConversationDtoAsync(newConversation, receiverUserId, senderUserId);
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
                SenderId = senderUser.Id,
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
                Content = message.Content,
                CreatedDate = message.SentDate,
                SenderId = message.SenderId,
                SenderUserName = senderUser.UserName,
                ConversationId = message.ConversationId,
            };

            await _hubService.SendMessageAsync(message.ConversationId, returnMessageDto); 
            await _hubService.SendMessageNotificationAsync(conversationId, returnMessageDto); 

            return ApiResponses<MessageDto>.Success(returnMessageDto);

        }

        private async Task<ConversationDto> ConversationToConversationDtoAsync(Conversation conversation, Guid responderId, Guid senderUserId)
        {
            return new ConversationDto
            {
                ConversationId = conversation.ConversationId,
                SenderUserId = senderUserId,
                ResponderUser = await GetResponderUserAsync(responderId),
                Messages = await GetConversationMessagesAsync(conversation)
            };
        }

        private async Task<ConversationDto> ConversationToConversationDtoWithLastMessageAsync(Conversation conversation, Guid responderId, Guid senderUserId)
        {
            return new ConversationDto
            {
                ConversationId = conversation.ConversationId,
                SenderUserId = senderUserId,
                ResponderUser = await GetResponderUserAsync(responderId),
                Messages = await GetConversationLastMessagesAsync(conversation)
            };
        }

        private async Task<UserInfoDto> GetResponderUserAsync(Guid RespondoerId)
        {
            return await _context.Users.Where(x => x.Id == RespondoerId)
                .AsNoTracking()
                .Select(x => new UserInfoDto
                {
                    Age = DateTime.Now.Year - x.BirthDate.Year,
                    GenderId = x.GenderId,
                    ProfilePictureUrl = x.ProfilePictureUrl,
                    UserName = x.UserName,
                    UserId = x.Id,

                })
                .FirstOrDefaultAsync();
        }

        private async Task<List<MessageDto>> GetConversationMessagesAsync(Conversation conversation)
        {
            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversation.ConversationId)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    Content = m.Content,
                    CreatedDate = m.SentDate,
                    ReadDate = m.ReadDate,
                    SenderId = m.SenderId,
                    ReceiverId = conversation.ResponderId,
                    IsRead = m.IsRead,
                    IsDeletedBySender = m.IsVisibleToInitiatorUser,
                    IsDeletedByReceiver = m.IsVisibleToResponderUser,
                    SenderIpAddress = m.IpAddress,
                    SenderDeviceInfo = m.DeviceInfo
                })
                .ToListAsync();

            return messages;
        }

        private async Task<List<MessageDto>> GetConversationLastMessagesAsync(Conversation conversation)
        {
            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversation.ConversationId)
                .OrderByDescending(m => m.SentDate) // son mesaj üstte
                .Select(m=> new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    Content = m.Content,
                    CreatedDate = m.SentDate,
                    ReadDate = m.ReadDate,
                    SenderId = m.SenderId,
                    ReceiverId = conversation.ResponderId,
                    IsRead = m.IsRead,
                    IsDeletedBySender = m.IsVisibleToInitiatorUser,
                    IsDeletedByReceiver = m.IsVisibleToResponderUser,
                    SenderIpAddress = m.IpAddress,
                    SenderDeviceInfo = m.DeviceInfo
                })
                .Take(1)
                .ToListAsync();
        }

    }
}
