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
        public const string SuperAdmin = nameof(UserRoleenum.SuperAdmin);
        public const string Admin = nameof(UserRoleenum.Admin);
        public const string Moderator = nameof(UserRoleenum.Moderator);
        public const string SuperUser = nameof(UserRoleenum.SuperUser);
        public const string User = nameof(UserRoleenum.User);
    }
}
