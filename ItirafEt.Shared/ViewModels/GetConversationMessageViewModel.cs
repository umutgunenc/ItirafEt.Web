using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Services.ClientServices.State;

namespace ItirafEt.Shared.ViewModels
{
    public class GetConversationMessageViewModel
    {
        public InfiniteScrollState<MessageViewModel> Model { get; set; }
        public Guid ConversationId { get; set; }
        public Guid ResponderUserId { get; set; }
    }
}
