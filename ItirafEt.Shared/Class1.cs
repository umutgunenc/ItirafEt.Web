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
        public const string SuperAdmin = nameof(UserRole.SuperAdmin);
        public const string Admin = nameof(UserRole.Admin);
        public const string Moderator = nameof(UserRole.Moderator);
        public const string SuperUser = nameof(UserRole.SuperUser);
        public const string User = nameof(UserRole.User);
    }
}
