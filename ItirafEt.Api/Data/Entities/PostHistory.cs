using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ItirafEt.Api.Data.Entities
{
    public class PostHistory
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [Required]
        [MaxLength(4096)]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(512)]
        public string DeviceInfo { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }

        [Required]
        public Guid OperationByUserId { get; set; }  

        [ForeignKey(nameof(OperationByUserId))]
        public virtual User User { get; set; }

    }
}
