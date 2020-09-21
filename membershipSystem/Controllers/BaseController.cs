using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using membershipSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace membershipSystem.Controllers
{
    public class BaseController : Controller
    {
        protected UserManager<AppUser> _userManager { get; set; }
        protected SignInManager<AppUser> _signInManager { get; set; }
        protected readonly IMapper _mapper;

        protected AppUser CurrentUser => _userManager.FindByNameAsync(User.Identity.Name).Result;
        public BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public void AddModelError(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
        }
    }
}
