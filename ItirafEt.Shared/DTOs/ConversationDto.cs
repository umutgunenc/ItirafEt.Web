using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public class ConversationDto
    {
        public Guid ConversationId { get; set; }
        public Guid InitiatorId { get; set; }
        public Guid ResponderId { get; set; }
        public string InitiatorUserName { get; set; }
        public string ResponderUserName { get; set; }

    }
}
