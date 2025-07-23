using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class InboxItemViewModel
    {
        public Guid ConversationId { get; set; }
        public string? SenderUserUserName { get; set; }
        public string? SenderUserProfileImageUrl { get; set; }
        public string? LastMessagePrewiew { get; set; }
        public DateTime LastMessageDate { get; set; }
        public int UnreadMessageCount { get; set; } 
    }
}
