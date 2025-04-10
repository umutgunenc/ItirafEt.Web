using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class Conversation
    {
        public Conversation()
        {
            Messages = new HashSet<Message>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public Guid InitiatorId { get; set; }  

        [Required]
        public Guid ResponderId { get; set; }

        [MaxLength(128)]
        public string? ConversationTitle { get; set; }

        [Required]
        public DateTime LastMessageDate { get; set; }

        [MaxLength(64)]
        public string? LastMessagePreview { get; set; } // Son mesajdan özet (64 karakter gibi)
        //public bool IsUser1SeenLastMessage { get; set; }
        //public bool IsUser2SeenLastMessage { get; set; }
        public bool IsDeletedByUser1 { get; set; }
        public bool IsDeletedByUser2 { get; set; }

        [ForeignKey(nameof(InitiatorId))]
        public virtual User Initiator { get; set; }

        [ForeignKey(nameof(ResponderId))]
        public virtual User Responder { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

    }
}