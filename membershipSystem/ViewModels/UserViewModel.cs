using membershipSystem.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace membershipSystem.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage ="Kullanıcı Adı Giriniz!")]
        [Display(Name ="Kullanıcı Adı")]
        public string UserName { get; set; }
       
        [Display(Name = "Telefon Numarası")]
        [RegularExpression(@"^(0(\d{3}) (\d{3}) (\d{2}) (\d{2}))$", ErrorMessage ="Telefon numarası doğru formatta değildir.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Geçerli Bir Email Adresi Giriniz!")]
        [Display(Name = "Email Adresi")]
        [EmailAddress(ErrorMessage ="Email Adresiniz Doğru Formatta Değildir!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Giriniz!")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Şehir")]
        public string City { get; set; }

        public string Picture { get; set; }

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Cinsiyet")]
        public Gender Gender { get; set; }
    }
}
