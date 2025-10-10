using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class CreateReportViewModel
    {

        [Required (ErrorMessage ="Lütfen Şikayet Adını Giriniz"), MaxLength(64,ErrorMessage ="Şikayet Adı Maksimum 64 karakter uzunluğunda olabilir.")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(128, ErrorMessage = "Şikayet Icon URLi Maksimum 128 karakter uzunluğunda olabilir.")]
        public string? IconUrl { get; set; }

    }
}
