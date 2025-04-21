using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Shared
{
    public static class RoleNames
    {
        public const string SuperAdmin = nameof(UserRoleEnum.SuperAdmin);
        public const string Admin = nameof(UserRoleEnum.Admin);
        public const string Moderator = nameof(UserRoleEnum.Moderator);
        public const string SuperUser = nameof(UserRoleEnum.SuperUser);
        public const string User = nameof(UserRoleEnum.User);
    }
}
