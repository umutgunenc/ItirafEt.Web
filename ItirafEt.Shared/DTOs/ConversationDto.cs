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
        public Guid SenderUserId { get; set; }
        public UserInfoDto ResponderUser { get; set; }
        public List<MessageDto> Messages { get; set; }

    }
}
