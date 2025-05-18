using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public class UserInfoDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public int GenderId { get; set; }
        public int Age { get; set; }
    }
}
