using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class MessageAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MessageId { get; set; }

        public string? PhotoUrl { get; set; }

        [ForeignKey(nameof(MessageId))]
        public virtual Message Message { get; set; }
    }
}
