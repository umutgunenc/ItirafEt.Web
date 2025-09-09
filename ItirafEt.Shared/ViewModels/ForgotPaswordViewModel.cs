using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class ForgotPaswordViewModel
    {

        [Required(ErrorMessage = "Kullanıcı Adınızı veye Email Adresinizi Giriniz.")]
        public string UserNameOrEmailAdres { get; set; }

        [JsonIgnore]
        public string? IpAddress { get; set; }

        [JsonIgnore]
        public string? DeviceInfo { get; set; }

    }
}
