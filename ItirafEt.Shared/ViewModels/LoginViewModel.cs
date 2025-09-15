using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="E-Mail Adresinizi veya Kullanıcı Adınızı Giriniz.")]
        public string EmailOrUserName { get; set; }

        [Required(ErrorMessage = "Şifrenizi Giriniz.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        //    ErrorMessage = "Şifre en az 8 karakter uzunluğunda ve büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
        public string Password { get; set; }

        [JsonIgnore]
        public string? IpAddress { get; set; }

        [JsonIgnore]
        public string? DeviceInfo { get; set; }
    }
}
