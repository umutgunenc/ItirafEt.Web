using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{

    public interface IMessageApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/message/SendMessage")]
        Task<ApiResponses<MessageDto>> SendMessageAsync(Guid conversationId, MessageDto messageDto);

        [Headers("Authorization: Bearer")]
        [Post("/api/message/GetConversation/")]
        Task<ApiResponses<ConversationDto>> GetConversationDtoAsync(Guid senderUserId, Guid receiverUserId);



    }
}
