﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using membershipSystem.Models;
using membershipSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace membershipSystem.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper):base(userManager,signInManager,null,mapper)
        {
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Member");
            }
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

                if (_userManager.IsEmailConfirmedAsync(user).Result==false)
                {
                    ModelState.AddModelError("", "Email adresiniz onaylanmamıştır.Lütfen e-postanızı kontrol ediniz.");

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
                if (_userManager.Users.Any(x => x.PhoneNumber == userViewModel.PhoneNumber))
                {
                    ModelState.AddModelError("", "Bu telefon numarası kayıtlıdır.");
                    return View(userViewModel);
                }

                AppUser appUser = _mapper.Map<UserViewModel, AppUser>(userViewModel);
                IdentityResult result = await _userManager.CreateAsync(appUser, userViewModel.Password);
                if (result.Succeeded)
                {
                    string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                    string link = Url.Action("ConfirmEmail", "Home", new
                    {
                        userId = appUser.Id,
                        token=confirmationToken
                    },protocol:HttpContext.Request.Scheme);

                    Helper.EmailConfirmation.SendMail(link, appUser.Email);

                    return RedirectToAction("LogIn");
                }
                else
                {
                    AddModelError(result);
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

                Helper.PasswordReset.PasswordResetSendMail(passwordResetLink,user.Email);
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
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Bir hata meydana geldi.Lütfen daha sonra tekrar deneyiniz.");
            }
            return View(passwordResetViewModel);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                ViewBag.status = "Email adresiniz onaylanmıştır.Login ekranından giriş yapabilirsiniz.";
            }
            else
            {
                ViewBag.status = "Bir hata meydana geldi.Lütfen daha sonra tekrar deneyiniz.";
            }
            return View();
        }

        public IActionResult FacebookLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("FacebookResponse", "Home", new { ReturnUrl = ReturnUrl });

            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook",RedirectUrl);

            return new ChallengeResult("Facebook", properties);
        }

        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });

            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Goole", RedirectUrl);

            return new ChallengeResult("Google", properties);
        }

        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/member")
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult loginResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

                if (loginResult.Succeeded)
                    return Redirect(ReturnUrl);
                else
                {
                    AppUser user = new AppUser();
                    user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                    if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;
                        userName = userName.Replace(' ', '-').ToLower() + ExternalUserId.Substring(0, 5).ToString();
                        user.UserName = userName;
                    }
                    else
                    {
                        user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    }
                    IdentityResult createResult = await _userManager.CreateAsync(user);
                    if (createResult.Succeeded)
                    {
                        IdentityResult addLoginResult = await _userManager.AddLoginAsync(user, info);
                       
                        if (addLoginResult.Succeeded)
                        {
                            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                            return Redirect(ReturnUrl);
                        }
                    }
                }
            }
            List<string> errors = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList();

            return View("Error", errors);
        }

        public IActionResult Error()
        {
            return View();
        }
    }   
}