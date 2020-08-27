using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace membershipSystem.ViewModels
{
    public class PasswordResetViewModel
    {
        [Display(Name = "Email Adresi")]
        [Required(ErrorMessage = "Email Alanı Gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
