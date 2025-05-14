using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Kullanıcı Adını Giriniz.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Şifrenizi Giriniz.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        //    ErrorMessage = "Şifre en az 8 karakter uzunluğunda ve büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifrenizi Tekrar Giriniz.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "E-posta adresinizi giriniz.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
        public string Email { get; set; }


        public bool isTermsAccepted { get; set; }

    }
}
