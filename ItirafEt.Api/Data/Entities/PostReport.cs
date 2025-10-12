using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.Data.Entities
{
    public class PostReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid ReportingUserId { get; set; }

        [ForeignKey(nameof(ReportingUserId))]
        public virtual User ReportingUser { get; set; }

        [Required]
        public int PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

        [Required]
        public int ReportTypeId { get; set; }

        [ForeignKey(nameof(ReportTypeId))]
        public virtual ReportType ReportType { get; set; }

        [MaxLength(1024)]
        public string? ReportExplanation { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public ReportStatusEnum Status { get; set; } = ReportStatusEnum.Pending;
        public Guid? ReviewedAdminId { get; set; }

        [ForeignKey(nameof(ReviewedAdminId))]
        public virtual User? ReviewedAdmin { get; set; }
        public DateTime? ReviewedDate { get; set; }

    }
}
