using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.Data.Entities
{
    public class RoleType
    {
        protected RoleType()
        {
            Users = new HashSet<User>();
        }

        public static readonly RoleType SuperAdmin = new RoleType( nameof(UserRoleEnum.SuperAdmin));
        public static readonly RoleType Admin = new RoleType( nameof(UserRoleEnum.Admin));
        public static readonly RoleType Moderator = new RoleType( nameof(UserRoleEnum.Moderator));
        public static readonly RoleType SuperUser = new RoleType( nameof(UserRoleEnum.SuperUser));
        public static readonly RoleType User = new RoleType( nameof(UserRoleEnum.User));

        public static IEnumerable<RoleType> List() => new[]
        {
            SuperAdmin,
            Admin,
            Moderator,
            SuperUser,
            User
        };

        [Key]
        [MaxLength(64)]
        public string Name { get; private set; }

        public virtual ICollection<User> Users { get; private set; }

        private RoleType(string name) : this()
        {
            Name = name;
        }


    }

}
