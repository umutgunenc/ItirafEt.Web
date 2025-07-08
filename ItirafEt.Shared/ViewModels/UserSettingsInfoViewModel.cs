using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class UserSettingsInfoViewModel
    {


        [Required(ErrorMessage = "Kullanıcı Adını Giriniz.")]
        [MinLength(3, ErrorMessage = "Kullanıcı adı minumum 3 karakter uzunluğunda olmalıdır.")]
        [MaxLength(64, ErrorMessage = "Kullanıcı adı maksimum 64 karakter uzunluğunda olmalıdır.")]
        public string UserName { get; set; }



        [Required(ErrorMessage = "E-posta adresinizi giriniz.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(256, ErrorMessage = "E-posta adresi maksimum 256 karakter uzunluğunda olmalıdır.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Doğum Tarihini Giriniz.")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Lütfen Cinsiyet Seçiniz.")]
        [Range(2, 3, ErrorMessage = "Lütfen Cinsiyet Seçiniz.")]
        public int GenderId { get; set; }
        public string? ProfileImageUrl { get; set; }

        public bool isProfilePrivate { get; set; } = false;


    }
}
