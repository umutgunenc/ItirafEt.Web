using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class ConversationViewModel
    {
        public Guid ConversationId { get; set; }
        public Guid SenderUserId { get; set; }
        public UserInfoViewModel ResponderUser { get; set; }
        public MessageViewModel? LastMessage { get; set; }

    }
}
