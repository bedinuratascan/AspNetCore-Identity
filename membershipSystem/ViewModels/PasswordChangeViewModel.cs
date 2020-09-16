using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace membershipSystem.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage ="Lütfen eski şifrenizi giriniz.")]
        [Display(Name ="Eski Şifreniz")]
        [DataType(DataType.Password)]
        [MinLength(4,ErrorMessage ="Şifreniz en az 4 karakterli olmalıdır.")]
        public string PasswordOld { get; set; }

        [Required(ErrorMessage = "Lütfen yeni şifrenizi giriniz.")]
        [Display(Name = "Yeni Şifreniz")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Yeni şifreniz en az 4 karakterli olmalıdır.")]
        public string PasswordNew { get; set; }

        [Required(ErrorMessage = "Lütfen onay yeni şifrenizi giriniz.")]
        [Display(Name = "Onay Yeni Şifreniz")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Onay yeni şifreniz en az 4 karakterli olmalıdır.")]
        [Compare("PasswordNew",ErrorMessage ="Yeni şifreniz ve onay şifreniz birbirinden farklıdır.")]
        public string PasswordConfirm { get; set; }
    }
}
