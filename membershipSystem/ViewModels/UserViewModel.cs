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
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Geçerli Bir Email Adresi Giriniz!")]
        [Display(Name = "Email Adresi")]
        [EmailAddress(ErrorMessage ="Email Adresiniz Doğru Formatta Değildir!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Giriniz!")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
