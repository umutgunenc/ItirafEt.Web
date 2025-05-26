using ItirafEt.Shared.ClientServices.State;
using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{

    public interface IMessageApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/message/SendMessage")]
        Task<ApiResponses<MessageDto>> SendMessageAsync(Guid conversationId, MessageDto messageDto);

        [Headers("Authorization: Bearer")]
        [Post("/api/message/GetConversationDto/")]
        Task<ApiResponses<ConversationDto>> GetConversationDtoAsync(Guid senderUserId, Guid receiverUserId);

        [Headers("Authorization: Bearer")]
        [Post("/api/message/GetConversation/")]
        Task<ApiResponses<ConversationDto>> GetConversationAsync(Guid ConversationId,Guid senderUserId);        
        
        [Headers("Authorization: Bearer")]
        [Get("/api/message/CanUserReadConversation/")]
        Task<ApiResponses<bool>> CanUserReadMessageApiAsync(Guid conversationId, Guid userId);
        
        [Headers("Authorization: Bearer")]
        [Get("/api/message/GetUserConversaions/")]
        Task<ApiResponses<List<ConversationDto>>> GetUserConversaionsAsync(Guid userId);

        [Headers("Authorization: Bearer")]
        [Post("/api/message/readMessage/")]
        Task<ApiResponses> ReadMessageAsync(Guid ConversationId, MessageDto messageDto);

        [Headers("Authorization: Bearer")]
        [Post("/api/message/getConversationMessages/")]
        Task<ApiResponses<InfiniteScrollState<MessageDto>>> GetConversationMessagesAsync(ConversationDto conversation, DateTime? nextBefore, int take);
    }
}

