using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ItirafEt.Api.Data.Entities
{
    public class UserLoginAttempt
    {

        [Key]
        public int Id { get; set; }

        public Guid? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public DateTime AttemptDate { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(512)]
        public string? DeviceInfo { get; set; }

        [Required]
        public bool IsSuccessful { get; set; }

    }
}