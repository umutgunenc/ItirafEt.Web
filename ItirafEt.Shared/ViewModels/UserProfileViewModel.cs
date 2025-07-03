using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; }
        public DateTime BirthDate { get; set; }
        public int GenderId { get; set; }
        public string? ProfilePictureUrl { get; set; }

    }
}
