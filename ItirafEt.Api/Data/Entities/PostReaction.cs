using ItirafEt.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ItirafEt.Api.Data.Entities
{
    public class PostReaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid ReactingUserId { get; set; }

        [ForeignKey(nameof(ReactingUserId))]
        public virtual User ReactingUser { get; set; }

        [Required]
        public int PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

        [Required]
        public ReactionType ReactionType { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ReportType? ReportType { get; set; }

        [MaxLength(1024)]
        public string? ReportExplanation { get; set; }


    }
}
