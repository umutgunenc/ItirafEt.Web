using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.Data.Entities
{
    public class ReportType
    {

        public ReportType()
        {
            CommentReports = new HashSet<CommentReport>();
            MessageReports = new HashSet<MessageReport>();
            PostReports = new HashSet<PostReport>();
        }

        [Key, Required, MaxLength(64)]
        public string Name { get; set; }

        public virtual ICollection<CommentReport> CommentReports { get; set; }
        public virtual ICollection<MessageReport> MessageReports { get; set; }
        public virtual ICollection<PostReport> PostReports { get; set; }

    }
}
