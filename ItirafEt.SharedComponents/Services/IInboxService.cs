using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.ViewModels;

namespace ItirafEt.SharedComponents.ClientServices
{
    public interface IInboxService
    {
        Task InitializeAsync(Guid userId);
        ref List<InboxViewModel> GetUserConversations();
        void Clear();
    }
}
