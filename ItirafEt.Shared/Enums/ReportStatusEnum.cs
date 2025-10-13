using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.Enums
{
    public enum ReportStatusEnum
    {
        [Display(Name = "Beklemede")]
        Pending = 0,
        [Display(Name = "Kabul Edildi")]
        Accepted,
        [Display(Name = "Reddedildi")]
        Ignored,
        [Display(Name = "Hepsi")]
        All
    }
}
