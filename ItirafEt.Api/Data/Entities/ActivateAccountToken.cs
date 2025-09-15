using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ItirafEt.Api.Data.Entities
{
    public class ActivateAccountToken
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
        public DateTime CreatedDate { get; set; }

        public DateTime? UsedDate { get; set; }

        [MaxLength(45)]
        public string? CreatedIpAddress { get; set; }

        [MaxLength(512)]
        public string? CreatedDeviceInfo { get; set; }

        [MaxLength(45)]
        public string? UsedIpAddress { get; set; }

        [MaxLength(512)]
        public string? UsedDeviceInfo { get; set; }
    }
}
