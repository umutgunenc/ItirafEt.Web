using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ItirafEt.Shared.ViewModels
{
    public class ChangeProfilePictureModel
    {
        public string UserId { get; set; }
        public IFormFile? Photo { get; set; }
        public string? PhotoUrl { get; set; } = string.Empty;
    }
}
