using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Shared.ViewModels
{
    public class ReportListViewModel
    {
        public int ReportCount { get; set; }
        public int? PostId { get; set; }
        public int? CommentId { get; set; }
        public Guid? ReportedUserId { get; set; }
        public string? ReportedUserName { get; set; }
        public ReportClassEnum ReportClass { get; set; }
        public ReportStatusEnum Status { get; set; }

    }
}
