using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Data.Entities
{
    [Index(nameof(ReactingUserId), nameof(CommentId), IsUnique = true)]
    public class CommentReaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid ReactingUserId { get; set; }

        [ForeignKey(nameof(ReactingUserId))]
        public virtual User ReactingUser { get; set; }

        [Required]
        public int CommentId { get; set; }

        [ForeignKey(nameof(CommentId))]
        public virtual Comment Comment { get; set; }


        public int? ReactionId { get; set; }

        [ForeignKey(nameof(ReactionId))]
        public ReactionType Reaction { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? ReportName { get; set; }

        [ForeignKey(nameof(ReportName))]
        public ReportType? ReportType { get; set; }

        [MaxLength(1024)]
        public string? ReportExplanation { get; set; }

    }
}