using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class BanUserViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BannedDateUntil { get; set; }
        public string? AdminUserName { get; set; }

    }
}
