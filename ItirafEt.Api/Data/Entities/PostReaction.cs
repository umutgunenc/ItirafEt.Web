using ItirafEt.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ItirafEt.Shared.DTOs;

namespace ItirafEt.Api.Data.Entities
{
    [Index(nameof(ReactingUserId), nameof(PostId), IsUnique = true)]

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

        public int ReactionTypeId { get; set; }

        [ForeignKey(nameof(ReactionTypeId))]
        public ReactionType ReactionType { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } 

    }
}
