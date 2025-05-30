using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class Message
    {
        public Message()
        {
            MessageReactions = new HashSet<MessageReaction>();
            MessageReports = new HashSet<MessageReport>();

        }

        [Key]
        public int Id { get; set; }

        public string? Content { get; set; }

        [Required]
        public Guid SenderId { get; set; }

        [Required]
        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(512)]
        public string DeviceInfo { get; set; }

        [Required]
        public DateTime SentDate { get; set; }

        public DateTime? ReadDate { get; set; }

        public bool IsRead { get; set; } = false;
        public bool IsVisibleToInitiatorUser { get; set; } = true;
        public bool IsVisibleToResponderUser { get; set; } = true;

        [Required]
        public Guid ConversationId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual User Sender { get; set; }

        [ForeignKey(nameof(ConversationId))]
        public virtual Conversation Conversation { get; set; }

        public string? PhotoUrl { get; set; }

        public virtual ICollection<MessageReaction> MessageReactions { get; set; }
        public virtual ICollection<MessageReport> MessageReports { get; set; }


    }


}

