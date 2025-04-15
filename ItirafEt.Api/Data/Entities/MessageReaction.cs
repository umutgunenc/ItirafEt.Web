using ItirafEt.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Data.Entities
{
    [Index(nameof(ReactingUserId), nameof(MessageId), IsUnique = true)]

    public class MessageReaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid ReactingUserId { get; set; }
        [ForeignKey(nameof(ReactingUserId))]
        public virtual User ReactingUser { get; set; }

        [Required]
        public int MessageId { get; set; }
        [ForeignKey(nameof(MessageId))]
        public virtual Message Message { get; set; }

        public int? ReactionTypeId { get; set; }
        [ForeignKey(nameof(ReactionTypeId))]
        public ReactionType ReactionType { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(1024)]
        public string? ReportExplanation { get; set; }
    }
}