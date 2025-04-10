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
        public Guid User1Id { get; set; }  

        [Required]
        public Guid User2Id { get; set; }

        [MaxLength(128)]
        public string? ConversationTitle { get; set; }

        [Required]
        public DateTime LastMessageDate { get; set; }

        [MaxLength(64)]
        public string? LastMessagePreview { get; set; } // Son mesajdan özet (64 karakter gibi)
        public bool IsUser1SeenLastMessage { get; set; }
        public bool IsUser2SeenLastMessage { get; set; }

        [ForeignKey(nameof(User1Id))]
        public virtual User User1 { get; set; }

        [ForeignKey(nameof(User2Id))]
        public virtual User User2 { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

    }
}