using ItirafEt.Shared.Services.ClientServices.State;
using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{

    public interface IMessageApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/message/SendMessage")]
        Task<ApiResponses<MessageViewModel>> SendMessageAsync(CreateMessageViewModel model);

        [Multipart]
        [Headers("Authorization: Bearer")]
        [Post("/api/message/SendMessageWithPhoto")]
        Task<ApiResponses<MessageViewModel>> SendMessageWithPhotoAsync(
            [AliasAs("ConversationId")] string conversationId,
            [AliasAs("Content")] string content,
            [AliasAs("SenderId")] string senderId,
            [AliasAs("ReceiverId")] string receiverId,
            [AliasAs("Photo")] StreamPart? photo,
            [AliasAs("Thumbnail")] StreamPart? thumb
        );

        [Headers("Authorization: Bearer")]
        [Post("/api/message/GetConversationDto")]
        Task<ApiResponses<ConversationViewModel>> GetConversationDtoAsync(Guid senderUserId, Guid receiverUserId);

        [Headers("Authorization: Bearer")]
        [Post("/api/message/GetConversation")]
        Task<ApiResponses<ConversationViewModel>> GetConversationAsync(Guid ConversationId,Guid senderUserId);        
        
        [Headers("Authorization: Bearer")]
        [Get("/api/message/CanUserReadConversation")]
        Task<ApiResponses<bool>> CanUserReadMessageApiAsync(Guid conversationId, Guid userId);
        
        //[Headers("Authorization: Bearer")]
        //[Get("/api/message/GetUserConversaions/")]
        //Task<ApiResponses<List<ConversationViewModel>>> GetUserConversaionsAsync(Guid userId);

        //[Headers("Authorization: Bearer")]
        //[Get("/api/message/photo")]
        //Task<Stream> GetPhotoAsync([Query] string photoId, [Query] string userId);


        //[Headers("Authorization: Bearer")]
        //[Get("/api/message/thumbnail")]
        //Task<Stream> GetThumbnailAsync([Query] string thumbnailId, [Query] string userId);

        [Headers("Authorization: Bearer")]
        [Post("/api/message/readMessage")]
        Task<ApiResponses> ReadMessageAsync(Guid ConversationId, MessageViewModel model);


        [Headers("Authorization: Bearer")]
        [Get("/api/message/getSignedPhotoUrl")]
        Task<ApiResponses<string>> GetSignedPhotoUrlAsync(string photoId, Guid userId);

        [Headers("Authorization: Bearer")]
        [Post("/api/message/getConversationMessages")]
        Task<ApiResponses<InfiniteScrollState<MessageViewModel>>> GetConversationMessagesAsync(GetConversationMessageViewModel model);


        [Headers("Authorization: Bearer")]
        [Get("/api/message/GetUserMessages")]
        Task<ApiResponses<List<InboxItemViewModel>>> GetUserMessagesAsync(Guid userId);
        
        [Headers("Authorization: Bearer")]
        [Get("/api/message/CheckUnreadMessages")]
        Task<ApiResponses<bool>> CheckUnreadMessagesAsync(Guid userId);
    }
}

