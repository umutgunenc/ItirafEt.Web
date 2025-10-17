using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Shared.ViewModels
{
    public class SendReportViewModel
    {

        public Guid ReportingUserId { get; set; }

        public int? PostId { get; set; }

        public int? ComemntId { get; set; }

        public Guid? ReportedUserId { get; set; }

        [Required(ErrorMessage = "Lütfen şikayet türü seçiniz.")]
        public int? ReportTypeId { get; set; }

        [MaxLength(1024, ErrorMessage = "Şikayet açıklması Maksimum 1024 karakter uzunluğunda olmalıdır.")]
        public string? ReportExplanation { get; set; }

        public int ReportClass { get; set; }

    }
}
