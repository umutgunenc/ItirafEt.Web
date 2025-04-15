using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ItirafEt.Api.Data.Entities
{
    public class MessageReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid ReportingUserId { get; set; }

        [ForeignKey(nameof(ReportingUserId))]
        public virtual User ReportingUser { get; set; }

        [Required]
        public int MessageId { get; set; }

        [ForeignKey(nameof(MessageId))]
        public virtual Message Message { get; set; }

        [Required]
        public int ReportTypeId { get; set; }

        [ForeignKey(nameof(ReportTypeId))]
        public virtual ReportType ReportType { get; set; }

        [MaxLength(1024)]
        public string? ReportExplanation { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
