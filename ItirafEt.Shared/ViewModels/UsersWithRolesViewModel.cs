using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Services.ClientServices.NewFolder;

namespace ItirafEt.Shared.ViewModels
{
    public class UsersWithRolesViewModel
    {
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string AssignedByUserName { get; set; }
    }

    public class SelectOptionForUserWithRoleViewModel : PageResult<UsersWithRolesViewModel>
    {
        public string? SearchedUserName { get; set; }
        public string SelectedRoleName { get; set; }
        public bool ShowAll { get; set; }

    }
}
