using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared;

namespace ItirafEt.Api.Data.Entities
{
    public class CommentReaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public int CommentId { get; set; }

        [ForeignKey(nameof(CommentId))]
        public virtual Comment Comment { get; set; }

        [Required]
        public ReactionType ReactionType { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ReportType? ReportType { get; set; }

        [MaxLength(1024)]
        public string? ReportExplanation { get; set; }

    }
}