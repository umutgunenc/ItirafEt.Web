using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class ChangeUserRoleViewModel
    {
        [Required(ErrorMessage = "Lüttfen kullanıcı adını veya kullanıcı ID sini giriniz.")]
        public string UserNameOrUserId { get; set; }
        public Guid AdminUserId { get; set; }
        public List<string> RolesNames { get; set; } = new();
        public string? SelectedRoleName { get; set; }
        public DateTime? ExpireDate { get; set; }
        public Guid AssignedByUserId { get; set; }

    }
}
