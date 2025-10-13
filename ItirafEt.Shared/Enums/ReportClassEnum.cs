using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.Enums
{
    public enum ReportClassEnum
    {
        [Display(Name = "Gönderi")]
        Post = 1,

        [Display(Name = "Yorum")]
        Comment,

        [Display(Name = "Kullanıcı")]
        User
    }
}
