using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Claims;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.ClientServices.State;
using ItirafEt.Shared.DTOs;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class MessageService
    {
        private readonly dbContext _context;
        private readonly MessageHubService _hubService; 
        private readonly IWebHostEnvironment _env;

        private readonly List<string> _allowedRoles = new()
        {
            UserRoleEnum.SuperAdmin.ToString(),
            UserRoleEnum.Admin.ToString(),
            UserRoleEnum.Moderator.ToString(),
            UserRoleEnum.SuperUser.ToString()
        };
        public MessageService(dbContext context, MessageHubService hubService, IWebHostEnvironment env)
        {
            _context = context;
            _hubService = hubService;
            _env = env;
        }

        public async Task<ApiResponses<List<ConversationDto>>> GetUserConversaionsAsync(Guid userId)
        {
            var conversations = await _context.Conversations
                .AsNoTracking()
                .Where(c => c.InitiatorId == userId || c.ResponderId == userId)
                .ToListAsync();

            if (conversations.Count == 0)
                return ApiResponses<List<ConversationDto>>.Fail("Mesajlaşma Bulunamadı.");

            var returnConversationsDto = new List<ConversationDto>();
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

        public async Task<ApiResponses<MessageDto>> SendMessageAsync(CreateMessageDto messageDto)
        {
            Guid.TryParse(messageDto.SenderId, out Guid senderId);
            Guid.TryParse(messageDto.ReceiverId, out Guid receiverId);
            Guid.TryParse(messageDto.ConversationId, out Guid conversationId);

            var isMessageValid = await IsMessageValidAsync(messageDto, senderId, receiverId, conversationId);
            if (!isMessageValid.Item1)
                return ApiResponses<MessageDto>.Fail(isMessageValid.Item2);

            var message = new Message
            {
                Content = messageDto.Content,
                ConversationId = conversationId,
                SenderId = senderId,
                SentDate = DateTime.Now,
                IsRead = false,
                IsVisibleToInitiatorUser = true,
                IsVisibleToResponderUser = true,
                ReadDate = null,
                IpAddress = messageDto.SenderIpAddress,
                DeviceInfo = messageDto.SenderDeviceInfo,

            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();


            var returnMessageDto = new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                CreatedDate = message.SentDate,
                SenderId = message.SenderId,
                SenderUserName = isMessageValid.Item2,
                ConversationId = message.ConversationId,
            };

            await _hubService.SendMessageAsync(message.ConversationId, returnMessageDto);
            await _hubService.SendMessageNotificationAsync(conversationId, returnMessageDto);

            return ApiResponses<MessageDto>.Success(returnMessageDto);

        }

        public async Task<ApiResponses<MessageDto>> SendMessageWithPhotoAsync(CreateMessageDto messageDto)
        {
            Guid.TryParse(messageDto.SenderId, out Guid senderId);
            Guid.TryParse(messageDto.ReceiverId, out Guid receiverId);
            Guid.TryParse(messageDto.ConversationId, out Guid conversationId);

            var isMessageValid = await IsMessageValidAsync(messageDto, senderId, receiverId, conversationId);
            if (!isMessageValid.Item1)
                return ApiResponses<MessageDto>.Fail(isMessageValid.Item2);

            if (messageDto.Photo == null)
                return ApiResponses<MessageDto>.Fail("Fotoğraf yüklenmedi.");


            List<string> allowedExtendions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };

            var extension = Path.GetExtension(messageDto.Photo.FileName).ToLowerInvariant();

            if (!allowedExtendions.Contains(extension))
                return ApiResponses<MessageDto>.Fail("Geçersiz dosya uzantısı. Sadece .jpg, .jpeg, .png ve .gif uzantılı dosyalar yüklenebilir.");


            if (messageDto.Photo.Length > 10 * 1024 * 1024) // 5 MB limit
                return ApiResponses<MessageDto>.Fail("Fotoğraf boyutu 10 MB'dan büyük olamaz.");


            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = messageDto.Content,
                PhotoUrl = messageDto.PhotoUrl,
                SentDate = DateTime.Now,
                IpAddress = messageDto.SenderIpAddress,
                DeviceInfo = messageDto.SenderDeviceInfo,
                IsRead = false
            };


            //var fileName = $"{Guid.NewGuid()}_{messageDto.Photo.FileName}";
            //var filePath = Path.Combine("wwwroot", "uploads", "messages", fileName);
            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await messageDto.Photo.CopyToAsync(stream);
            //}
            //messageDto.PhotoUrl = $"/uploads/messages/{fileName}";


            //var message = new Message
            //{
            //    Content = messageDto.Content,
            //    ConversationId = conversationId,
            //    SenderId = senderId,
            //    SentDate = DateTime.Now,
            //    IsRead = false,
            //    IsVisibleToInitiatorUser = true,
            //    IsVisibleToResponderUser = true,
            //    ReadDate = null,
            //    IpAddress = messageDto.SenderIpAddress,
            //    DeviceInfo = messageDto.SenderDeviceInfo,
            //    PhotoUrl = messageDto.PhotoUrl

            //};
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();


            var returnMessageDto = new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                CreatedDate = message.SentDate,
                SenderId = message.SenderId,
                SenderUserName = isMessageValid.Item2,
                ConversationId = message.ConversationId,
                PhotoUrl = message.PhotoUrl,
            };

            await _hubService.SendMessageAsync(message.ConversationId, returnMessageDto);
            await _hubService.SendMessageNotificationAsync(message.ConversationId, returnMessageDto);

            return ApiResponses<MessageDto>.Success(returnMessageDto);

        }

        //public async Task<ApiResponses> GetMessagePhotoAsync(MessageDto dto,string fileName)
        //{
        //    if (string.IsNullOrEmpty(dto.PhotoUrl))
        //        return ApiResponses.Fail("Fotoğraf URL'si boş veya geçersiz.");

        //    if (dto.ConversationId == Guid.Empty)
        //        return ApiResponses.Fail("Geçersiz Mesajlaşma ID'si.");

        //    var conversation = await _context.Conversations
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(c => c.ConversationId == dto.ConversationId);

        //    if (conversation == null)
        //        return ApiResponses.Fail("Mesajlaşma Bulunamadı.");

        //    if (conversation.InitiatorId != dto.SenderId && conversation.ResponderId != dto.ReceiverId)
        //        return ApiResponses.Fail("Mesajlaşma Bulunamadı.");

        //    var message = await _context.Messages
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(m => m.PhotoUrl.Contains(fileName));

        //    if (message == null)
        //        return ApiResponses.Fail("Mesajlaşma Bulunamadı.");

        //    var path = Path.Combine(_env.ContentRootPath, "PrivateFiles", "messages", filename);

        //    if (!System.IO.File.Exists(path))
        //        return NotFound();

        //    var mime = "image/jpeg"; // MIME türünü uzantıya göre belirleyebilirsin
        //    var bytes = await System.IO.File.ReadAllBytesAsync(path);
        //    return File(bytes, mime);


        //}


        public async Task<IResult> GetMessagePhotoAsync(string fileName, ClaimsPrincipal user)
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
                return Results.Unauthorized();

            var message = await _context.Messages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.PhotoUrl.EndsWith(fileName));

            if (message == null)
                return Results.NotFound();

            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationId == message.ConversationId);

            if (conversation == null)
                return Results.NotFound();

            if (conversation.InitiatorId != userId && conversation.ResponderId != userId)
                return Results.Forbid();

            var path = Path.Combine(_env.ContentRootPath, "PrivateFiles", "messages", conversation.ConversationId.ToString(),message.PhotoUrl);

            if (!System.IO.File.Exists(path))
                return Results.NotFound();

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out string? mime))
                mime = "application/octet-stream";

            var bytes = await System.IO.File.ReadAllBytesAsync(path);
            return Results.File(bytes, mime);
        }

        private static string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
        public async Task<ApiResponses> ReadMessageAsync(Guid conversationId, MessageDto messageDto)
        {
            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
            if (conversation == null)
                return ApiResponses.Fail("Mesajlaşma Bulunamadı.");
            if (conversation.InitiatorId != messageDto.SenderId && conversation.ResponderId != messageDto.SenderId)
                return ApiResponses.Fail("Mesajlaşma Bulunamadı.");

            var message = await _context.Messages
                .Where(m => m.Id == messageDto.Id)
                .FirstOrDefaultAsync();

            if (message == null)
                return ApiResponses.Fail("Mesaj Bulunamadı.");

            if (message.IsRead)
                return ApiResponses.Fail("Mesaj zaten okunmuş.");


            message.ReadDate = DateTime.Now;
            message.IsRead = true;
            _context.Update(message);
            await _context.SaveChangesAsync();

            messageDto.ReadDate = message.ReadDate;
            messageDto.IsRead = message.IsRead;
            await _hubService.ReadMessageAsync(conversationId, messageDto);

            return ApiResponses.Success();

        }

        private async Task<ConversationDto> ConversationToConversationDtoAsync(Conversation conversation, Guid responderId, Guid senderUserId)
        {
            return new ConversationDto
            {
                ConversationId = conversation.ConversationId,
                SenderUserId = senderUserId,
                ResponderUser = await GetResponderUserAsync(responderId)
            };
        }

        private async Task<ConversationDto> ConversationToConversationDtoWithLastMessageAsync(Conversation conversation, Guid responderId, Guid senderUserId)
        {
            return new ConversationDto
            {
                ConversationId = conversation.ConversationId,
                SenderUserId = senderUserId,
                ResponderUser = await GetResponderUserAsync(responderId),
                LastMessage = await GetConversationLastMessagesAsync(conversation)
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

        private async Task<MessageDto?> GetConversationLastMessagesAsync(Conversation conversation)
        {
            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversation.ConversationId)
                .OrderByDescending(m => m.SentDate)
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
                .FirstOrDefaultAsync();

        }

        public async Task<ApiResponses<InfiniteScrollState<MessageDto>>> GetConversationMessagesAsync(ConversationDto conversation, DateTime? nextBefore, int take)
        {
            var isThereConversation = await _context.Conversations
                .AsNoTracking()
                .AnyAsync(c => c.ConversationId == conversation.ConversationId);


            if (!isThereConversation)
                return ApiResponses<InfiniteScrollState<MessageDto>>.Fail("Mesajlaşma Bulunamadı.");

            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversation.ConversationId && m.SentDate < nextBefore.Value)
                .OrderByDescending(m => m.SentDate)
                .Take(take)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    Content = m.Content,
                    CreatedDate = m.SentDate,
                    ReadDate = m.ReadDate,
                    SenderId = m.SenderId,
                    ReceiverId = conversation.ResponderUser.UserId,
                    IsRead = m.IsRead,
                    IsDeletedBySender = m.IsVisibleToInitiatorUser,
                    IsDeletedByReceiver = m.IsVisibleToResponderUser,
                    PhotoUrl = m.PhotoUrl,

                })
                .ToListAsync();


            //messages.Reverse(); 

            var hasMore = messages.Count == take;
            var nextBeforeDateTime = hasMore ? messages.Last().CreatedDate : (DateTime?)null;
            var scrollStateMessageList = new InfiniteScrollState<MessageDto>
            {
                Items = messages,
                HasMore = hasMore,
                NextBefore = nextBeforeDateTime
            };

            return ApiResponses<InfiniteScrollState<MessageDto>>.Success(scrollStateMessageList);

        }


        public async Task<(bool, string)> IsMessageValidAsync(CreateMessageDto messageDto, Guid senderId, Guid receiverId, Guid conversationId)
        {

            var senderUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == senderId);

            if (senderUser == null)
                return (false, "Gönderici Kullanıcı Bulunamadı.");

            var receiverUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == receiverId);
            if (receiverUser == null)
                return (false, "Alıcı Kullanıcı Bulunamadı.");

            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation == null)
                return (false, "Mesaj gönderilemedi.");

            if (!((conversation.InitiatorId == senderId && conversation.ResponderId == receiverId) || (conversation.InitiatorId == receiverId && conversation.ResponderId == senderId)))
                return (false, "Mesaj gönderilemedi.");


            return (true, senderUser.UserName);
        }

    }
}
