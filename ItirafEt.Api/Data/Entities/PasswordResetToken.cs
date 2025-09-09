using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(64)]
        public string TokenHash { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public DateTime ExpTime { get; set; }

        [Required]
        public bool IsUsed { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(512)]
        public string? DeviceInfo { get; set; }

    }
}
