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
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
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

        public async Task<ApiResponses<List<ConversationViewModel>>> GetUserConversaionsAsync(Guid userId)
        {
            var conversations = await _context.Conversations
                .AsNoTracking()
                .Where(c => c.InitiatorId == userId || c.ResponderId == userId)
                .ToListAsync();

            if (conversations.Count == 0)
                return ApiResponses<List<ConversationViewModel>>.Fail("Mesajlaşma Bulunamadı.");

            var returnConversationsDto = new List<ConversationViewModel>();
            foreach (var conversation in conversations)
            {
                var responderUserId = conversation.InitiatorId == userId ? conversation.ResponderId : conversation.InitiatorId;
                var conversationDto = await ConversationToConversationModelWithLastMessageAsync(conversation, responderUserId, userId);
                returnConversationsDto.Add(conversationDto);
            }


            return ApiResponses<List<ConversationViewModel>>.Success(returnConversationsDto);
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
        public async Task<ApiResponses<ConversationViewModel>> GetConversationAsync(Guid conversationId, Guid senderUserId)
        {
            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation == null)
                return ApiResponses<ConversationViewModel>.Fail("Mesajlaşma Bulunamadı");
            var receiverUserId = conversation.InitiatorId == senderUserId ? conversation.ResponderId : conversation.InitiatorId;
            var conversationDto = await ConversationToConversationModelAsync(conversation, receiverUserId, senderUserId);

            return ApiResponses<ConversationViewModel>.Success(conversationDto);

        }

        public async Task<ApiResponses<ConversationViewModel>> GetConversationDtoAsync(Guid senderUserId, Guid receiverUserId)
        {
            var senderUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == senderUserId);

            if (senderUser == null)
                return ApiResponses<ConversationViewModel>.Fail("Gönderici Kullanıcı Bulunamadı.");

            var receiverUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == receiverUserId);
            if (receiverUser == null)
                return ApiResponses<ConversationViewModel>.Fail("Alıcı Kullanıcı Bulunamadı.");

            if (senderUserId == receiverUserId)
                return ApiResponses<ConversationViewModel>.Fail("Kendinize mesaj gönderemezsiniz.");

            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    (c.InitiatorId == senderUserId && c.ResponderId == receiverUserId)
                    ||
                    (c.InitiatorId == receiverUserId && c.ResponderId == senderUserId)
                );

            if (conversation != null)
            {
                var conversationDto = await ConversationToConversationModelAsync(conversation, receiverUserId, senderUserId);
                return ApiResponses<ConversationViewModel>.Success(conversationDto);
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

                    var conversationDto = await ConversationToConversationModelAsync(newConversation, receiverUserId, senderUserId);
                    return ApiResponses<ConversationViewModel>.Success(conversationDto);
                }
                else
                    return ApiResponses<ConversationViewModel>.Fail("Mesaj gönderebilmek için super kullanıcı olmalısınız.");
            }
        }

        public async Task<ApiResponses<MessageViewModel>> SendMessageAsync(CreateMessageViewModel model)
        {
            Guid.TryParse(model.SenderId, out Guid senderId);
            Guid.TryParse(model.ReceiverId, out Guid receiverId);
            Guid.TryParse(model.ConversationId, out Guid conversationId);

            var isMessageValid = await IsMessageValidAsync(model, senderId, receiverId, conversationId);
            if (!isMessageValid.Item1)
                return ApiResponses<MessageViewModel>.Fail(isMessageValid.Item2);

            var message = new Message
            {
                Content = model.Content,
                ConversationId = conversationId,
                SenderId = senderId,
                SentDate = DateTime.Now,
                IsRead = false,
                IsVisibleToInitiatorUser = true,
                IsVisibleToResponderUser = true,
                ReadDate = null,
                IpAddress = model.SenderIpAddress,
                DeviceInfo = model.SenderDeviceInfo,

            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();


            var returnModel = new MessageViewModel
            {
                Id = message.Id,
                Content = message.Content,
                CreatedDate = message.SentDate,
                SenderId = message.SenderId,
                SenderUserName = isMessageValid.Item2,
                ConversationId = message.ConversationId,
            };

            await _hubService.SendMessageAsync(message.ConversationId, returnModel);
            await _hubService.SendMessageNotificationAsync(conversationId, returnModel);

            return ApiResponses<MessageViewModel>.Success(returnModel);

        }

        public async Task<ApiResponses<MessageViewModel>> SendMessageWithPhotoAsync(CreateMessageViewModel model)
        {
            Guid.TryParse(model.SenderId, out Guid senderId);
            Guid.TryParse(model.ReceiverId, out Guid receiverId);
            Guid.TryParse(model.ConversationId, out Guid conversationId);

            var isMessageValid = await IsMessageValidAsync(model, senderId, receiverId, conversationId);
            if (!isMessageValid.Item1)
                return ApiResponses<MessageViewModel>.Fail(isMessageValid.Item2);

            if (model.Photo == null)
                return ApiResponses<MessageViewModel>.Fail("Fotoğraf yüklenmedi.");


            List<string> allowedExtendions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };

            var extension = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();

            if (!allowedExtendions.Contains(extension))
                return ApiResponses<MessageViewModel>.Fail("Geçersiz dosya uzantısı. Sadece .jpg, .jpeg, .png ve .gif uzantılı dosyalar yüklenebilir.");


            if (model.Photo.Length > 10 * 1024 * 1024) // 5 MB limit
                return ApiResponses<MessageViewModel>.Fail("Fotoğraf boyutu 10 MB'dan büyük olamaz.");


            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = model.Content,
                PhotoUrl = model.PhotoUrl,
                SentDate = DateTime.Now,
                IpAddress = model.SenderIpAddress,
                DeviceInfo = model.SenderDeviceInfo,
                IsRead = false
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();


            var returnModel = new MessageViewModel
            {
                Id = message.Id,
                Content = message.Content,
                CreatedDate = message.SentDate,
                SenderId = message.SenderId,
                SenderUserName = isMessageValid.Item2,
                ConversationId = message.ConversationId,
                PhotoUrl = message.PhotoUrl,
            };

            await _hubService.SendMessageAsync(message.ConversationId, returnModel);
            await _hubService.SendMessageNotificationAsync(message.ConversationId, returnModel);

            return ApiResponses<MessageViewModel>.Success(returnModel);

        }

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

        public async Task<ApiResponses> ReadMessageAsync(Guid conversationId, MessageViewModel model)
        {
            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
            if (conversation == null)
                return ApiResponses.Fail("Mesajlaşma Bulunamadı.");
            if (conversation.InitiatorId != model.SenderId && conversation.ResponderId != model.SenderId)
                return ApiResponses.Fail("Mesajlaşma Bulunamadı.");

            var message = await _context.Messages
                .Where(m => m.Id == model.Id)
                .FirstOrDefaultAsync();

            if (message == null)
                return ApiResponses.Fail("Mesaj Bulunamadı.");

            if (message.IsRead)
                return ApiResponses.Fail("Mesaj zaten okunmuş.");


            message.ReadDate = DateTime.Now;
            message.IsRead = true;
            _context.Update(message);
            await _context.SaveChangesAsync();

            model.ReadDate = message.ReadDate;
            model.IsRead = message.IsRead;
            await _hubService.ReadMessageAsync(conversationId, model);

            return ApiResponses.Success();

        }

        private async Task<ConversationViewModel> ConversationToConversationModelAsync(Conversation conversation, Guid responderId, Guid senderUserId)
        {
            return new ConversationViewModel
            {
                ConversationId = conversation.ConversationId,
                SenderUserId = senderUserId,
                ResponderUser = await GetResponderUserAsync(responderId)
            };
        }

        private async Task<ConversationViewModel> ConversationToConversationModelWithLastMessageAsync(Conversation conversation, Guid responderId, Guid senderUserId)
        {
            return new ConversationViewModel
            {
                ConversationId = conversation.ConversationId,
                SenderUserId = senderUserId,
                ResponderUser = await GetResponderUserAsync(responderId),
                LastMessage = await GetConversationLastMessagesAsync(conversation)
            };
        }

        private async Task<UserInfoViewModel> GetResponderUserAsync(Guid RespondoerId)
        {
            return await _context.Users.Where(x => x.Id == RespondoerId)
                .AsNoTracking()
                .Select(x => new UserInfoViewModel
                {
                    Age = DateTime.Now.Year - x.BirthDate.Year,
                    GenderId = x.GenderId,
                    ProfilePictureUrl = x.ProfilePictureUrl,
                    UserName = x.UserName,
                    UserId = x.Id,

                })
                .FirstOrDefaultAsync();
        }

        private async Task<MessageViewModel?> GetConversationLastMessagesAsync(Conversation conversation)
        {
            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversation.ConversationId)
                .OrderByDescending(m => m.SentDate)
                .Select(m => new MessageViewModel
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

        public async Task<ApiResponses<InfiniteScrollState<MessageViewModel>>> GetConversationMessagesAsync(ConversationViewModel conversation, DateTime? nextBefore, int take)
        {
            var isThereConversation = await _context.Conversations
                .AsNoTracking()
                .AnyAsync(c => c.ConversationId == conversation.ConversationId);


            if (!isThereConversation)
                return ApiResponses<InfiniteScrollState<MessageViewModel>>.Fail("Mesajlaşma Bulunamadı.");

            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversation.ConversationId && m.SentDate < nextBefore.Value)
                .OrderByDescending(m => m.SentDate)
                .Take(take)
                .Select(m => new MessageViewModel
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
            var scrollStateMessageList = new InfiniteScrollState<MessageViewModel>
            {
                Items = messages,
                HasMore = hasMore,
                NextBefore = nextBeforeDateTime
            };

            return ApiResponses<InfiniteScrollState<MessageViewModel>>.Success(scrollStateMessageList);

        }

        public async Task<(bool, string)> IsMessageValidAsync(CreateMessageViewModel messageDto, Guid senderId, Guid receiverId, Guid conversationId)
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

        public async Task<ApiResponses<List<InboxViewModel>>> GetUserMessagesAsync(Guid userId)
        {
            var currentUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (currentUser == null)
                return ApiResponses<List<InboxViewModel>>.Fail("Kullanıcı bulunamadı.");

            var conversations = await _context.Conversations
                .AsNoTracking()
                .Include(c => c.Initiator)
                .Include(c => c.Responder)
                .Where(c => c.InitiatorId == userId || c.ResponderId == userId)
                .ToListAsync();

            var conversationIds = conversations.Select(c => c.ConversationId).ToList();

            var lastMessages = await _context.Messages
                .AsNoTracking()
                .Where(m => conversationIds.Contains(m.ConversationId))
                .GroupBy(m => m.ConversationId)
                .Select(g => g.OrderByDescending(m => m.SentDate).FirstOrDefault())
                .ToListAsync();

            var unreadCounts = await _context.Messages
                .AsNoTracking()
                .Where(m => conversationIds.Contains(m.ConversationId) && !m.IsRead && m.SenderId != userId)
                .GroupBy(m => m.ConversationId)
                .Select(g => new
                {
                    ConversationId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var inboxDtos = conversations
                .Where(c => lastMessages.Any(m => m.ConversationId == c.ConversationId)) 
                .Select(c =>
                {
                    var partnerUser = c.InitiatorId == userId ? c.Responder : c.Initiator;
                    var lastMessage = lastMessages.FirstOrDefault(m => m.ConversationId == c.ConversationId);
                    var unreadCount = unreadCounts.FirstOrDefault(u => u.ConversationId == c.ConversationId)?.Count ?? 0;

                    return new InboxViewModel
                    {
                        ConversationId = c.ConversationId,
                        SenderUserUserName = partnerUser.UserName,
                        SenderUserProfileImageUrl = partnerUser.ProfilePictureUrl,
                        LastMessagePrewiew = lastMessage?.Content ?? "",
                        LastMessageDate = lastMessage?.SentDate ?? DateTime.MinValue,
                        UnreadMessageCount = unreadCount
                    };
                })
                .OrderByDescending(x => x.LastMessageDate)
                .ToList();

            if (!inboxDtos.Any())
                return ApiResponses<List<InboxViewModel>>.Fail("Mesaj Kutunuz Boş");

            return ApiResponses<List<InboxViewModel>>.Success(inboxDtos);
        }

    }
}
