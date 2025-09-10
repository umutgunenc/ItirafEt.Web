using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class ChangeUserPasswordViewModel
    {
        public Guid UserId { get; set; }


        [Required(ErrorMessage = "Yeni Şifrenizi Giriniz.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "Şifre en az 8 karakter uzunluğunda ve büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Yeni Şifrenizi Tekrar Giriniz")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string NewPasswordConfirm { get; set; }
        public string Token { get; set; }


    }
}
