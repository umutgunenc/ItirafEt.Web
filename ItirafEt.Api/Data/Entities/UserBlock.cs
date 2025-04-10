using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ItirafEt.Api.Data.Entities
{
    public class UserBlock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid BlockerUserId { get; set; }

        [Required]
        public Guid BlockedUserId { get; set; }

        [Required]
        public DateTime BlockedDate { get; set; }

        [ForeignKey(nameof(BlockerUserId))]
        public virtual User BlockerUser { get; set; }

        [ForeignKey(nameof(BlockedUserId))]
        public virtual User BlockedUser { get; set; }
    }
}
