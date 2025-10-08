using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class Conversation
    {
        public Conversation()
        {
            Messages = new HashSet<Message>();
            MessageReports = new HashSet<MessageReport>();
        }

        [Key]
        public Guid ConversationId { get; set; }

        [Required]
        public Guid InitiatorId { get; set; }  

        [Required]
        public Guid ResponderId { get; set; }

        public bool IsDeletedByUser1 { get; set; }
        public bool IsDeletedByUser2 { get; set; }

        [ForeignKey(nameof(InitiatorId))]
        public virtual User Initiator { get; set; }

        [ForeignKey(nameof(ResponderId))]
        public virtual User Responder { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<MessageReport> MessageReports { get; set; }


    }
}