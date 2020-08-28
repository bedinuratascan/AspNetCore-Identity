using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using membershipSystem.Models;
using membershipSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace membershipSystem.Controllers
{
    public class HomeController : Controller
    {
        public UserManager<AppUser> _userManager { get; set; }
        public SignInManager<AppUser> _signInManager { get; set; }
        private readonly IMapper _mapper;
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LogIn(string ReturnUrl)
        {
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel userLogin)
        {
            AppUser user = await _userManager.FindByEmailAsync(userLogin.Email);
            if (user != null)
            {
                if (await _userManager.IsLockedOutAsync(user))
                {
                    ModelState.AddModelError("", "Hesabınız bir süreliğine kitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");

                    return View(userLogin);
                }
                await _signInManager.SignOutAsync();
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, userLogin.Password, userLogin.RememberMe, false);
                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    if (TempData["ReturnUrl"] != null)
                    {
                        return Redirect(TempData["ReturnUrl"].ToString());
                    }
                    return RedirectToAction("Index", "Member");
                }
                else
                {
                    await _userManager.AccessFailedAsync(user);
                    int fail = await _userManager.GetAccessFailedCountAsync(user);

                    ModelState.AddModelError("", $"{fail} kez başarısız giriş.");

                    if (fail == 3)
                    {
                        await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(30)));
                        ModelState.AddModelError("", "Hesabınız 3 başarısız girişten dolayı 30 dk süreyle kitlenmiştir.Lütfen daha sonra tekrar deneyiniz.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email adresiniz veya şifreniz yanlış");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Bu email adresine kayıtlı kullanıcı bulunamamıştır.");
            }
            return View(userLogin);
        }

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = _mapper.Map<UserViewModel, AppUser>(userViewModel);
                IdentityResult result = await _userManager.CreateAsync(appUser, userViewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("LogIn");
                }
                else
                {
                    foreach (IdentityError item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(userViewModel);
        }

        public IActionResult ResetPassword()
        {
            return View();

        }
        [HttpPost]
        public IActionResult ResetPassword(PasswordResetViewModel passwordResetViewModel)
        {
            AppUser user = _userManager.FindByEmailAsync(passwordResetViewModel.Email).Result;
            if (user != null)
            {
                string passwordResetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;

                string passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
                {
                    userId = user.Id,
                    token=passwordResetToken

                }, HttpContext.Request.Scheme) ;

                Helper.PasswordReset.PasswordResetSendMail(passwordResetLink);
                ViewBag.status = "Success";
            }
            else
            {
                ModelState.AddModelError("", "Kayıtlı e-mail adresi bulunamamıştır.");
            }
            return View(passwordResetViewModel);
        }

        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("Password")]PasswordResetViewModel passwordResetViewModel)
        {
            string userId = TempData["userId"].ToString();
            string token = TempData["token"].ToString();

            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                IdentityResult result= await _userManager.ResetPasswordAsync(user, token, passwordResetViewModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    ViewBag.status = "success";
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Bir hata meydana geldi.Lütfen daha sonra tekrar deneyiniz.");
            }
            return View(passwordResetViewModel);
        }
    }   
}