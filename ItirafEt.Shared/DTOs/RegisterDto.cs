using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.CustomValidationAttribute;

namespace ItirafEt.Shared.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Kullanıcı Adını Giriniz.")]
        [MinLength(3, ErrorMessage = "Kullanıcı adı minumum 3 karakter uzunluğunda olmalıdır.")]
        [MaxLength(64, ErrorMessage = "Kullanıcı adı maksimum 64 karakter uzunluğunda olmalıdır.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Şifrenizi Giriniz.")]
        //[MinLength(8, ErrorMessage = "Şifre minimum 8 karakter uzunluğunda olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Şifre en az 8 karakter uzunluğunda ve büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifrenizi Tekrar Giriniz.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "E-posta adresinizi giriniz.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(256, ErrorMessage = "E-posta adresi maksimum 256 karakter uzunluğunda olmalıdır.")]
        public string Email { get; set; }

        [MustBeTrue(ErrorMessage = "Kullanım Koşulları ve Gizlilik Politikasını kabul etmelisiniz.")]
        public bool isTermsAccepted { get; set; }

        [Required(ErrorMessage = "Doğum Tarihini Giriniz.")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Lütfen Cinsiyet Seçiniz.")]
        [Range(2, 3, ErrorMessage = "Lütfen Cinsiyet Seçiniz.")]
        public int? GenderId { get; set; }

    }
}
