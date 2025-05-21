using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.DTOs;

namespace ItirafEt.SharedComponents.ClientServices
{
    public interface IConversationService
    {
        Task InitializeAsync(Guid userId);
        List<ConversationDto> GetUserConversations();
        void Clear();
    }
}
