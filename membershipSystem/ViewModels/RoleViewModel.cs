using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace membershipSystem.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Display(Name ="Rol")]
        [Required(ErrorMessage ="Rol ismi gereklidir.")]
        public string Name { get; set; }
    }
}
