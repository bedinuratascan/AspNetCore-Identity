using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace membershipSystem.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Email Adresi")]
        [Required(ErrorMessage = "Email Alanı Gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Alanı Gereklidir.")]
        [Display(Name = "Şifreniz")]
        [DataType(DataType.Password)]
        [MinLength(4,ErrorMessage ="Şifreniz en az 4 karakterli olmalıdır.")]
        public string Password { get; set; }
    }
}
