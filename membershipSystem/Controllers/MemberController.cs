using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using membershipSystem.Models;
using membershipSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace membershipSystem.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        public UserManager<AppUser> _userManager { get; set; }
        public SignInManager<AppUser> _signInManager { get; set; }
        private readonly IMapper _mapper;
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            AppUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            UserViewModel userViewModel = _mapper.Map<AppUser, UserViewModel>(user);

            return View(userViewModel);
        }
    }
}
