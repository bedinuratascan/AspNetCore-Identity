using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using membershipSystem.Enums;
using membershipSystem.Models;
using membershipSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace membershipSystem.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper):base(userManager,signInManager,null,mapper)
        {
        }

        public IActionResult Index()
        {
            AppUser user = CurrentUser;

            UserViewModel userViewModel = _mapper.Map<AppUser, UserViewModel>(user);

            return View(userViewModel);
        }

        public IActionResult UserEdit()
        {
            AppUser user = CurrentUser;

            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

            UserViewModel userViewModel = _mapper.Map<AppUser, UserViewModel>(user);

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel userViewModel, IFormFile userPicture)
        {
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                AppUser user = CurrentUser;

                string phone = _userManager.GetPhoneNumberAsync(user).Result;
                if (phone != userViewModel.PhoneNumber)
                {
                    if (_userManager.Users.Any(x => x.PhoneNumber == userViewModel.PhoneNumber))
                    {
                        ModelState.AddModelError("", "Bu telefon numarası başka bir kullanıcı tarafından kullanılmaktadır.");
                        return View(userViewModel);
                    }
                }

                ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

                if (userPicture!=null && userPicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture",fileName);

                    using(var stream=new FileStream(path, FileMode.Create))
                    {
                        await userPicture.CopyToAsync(stream);
                        user.Picture = "/UserPicture/" + fileName;
                    }
                }

                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.PhoneNumber = userViewModel.PhoneNumber;
                user.Birthday = userViewModel.Birthday;
                user.City = userViewModel.City;
                user.Gender =(int)userViewModel.Gender;

                IdentityResult result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(user, true);
                    ViewBag.success = "true";
                }
                else
                {
                    AddModelError(result);
                }
            }
            return View(userViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = CurrentUser;
                if (user != null)
                {
                    bool exist = _userManager.CheckPasswordAsync(user, passwordChangeViewModel.PasswordOld).Result;

                    if (exist)
                    {
                        IdentityResult result = _userManager.ChangePasswordAsync(user, passwordChangeViewModel.PasswordOld, passwordChangeViewModel.PasswordNew).Result;

                        if (result.Succeeded)
                        {
                            _userManager.UpdateSecurityStampAsync(user);
                            _signInManager.SignOutAsync();
                            _signInManager.PasswordSignInAsync(user, passwordChangeViewModel.PasswordNew, true,false);
                            ViewBag.success = "true";
                        }
                        else
                        {
                            AddModelError(result);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Eski Şifreniz Yanlış");
                    }
                }
            }
            return View(passwordChangeViewModel);
        }

        public void LogOut()
        {
            _signInManager.SignOutAsync();
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            if (ReturnUrl.Contains("ViolencePage"))
            {
                ViewBag.message = "Erişmeye çalıştığınız sayfada şiddet içerikli ögeler bulunduğundan dolayı 15 yaşından büyük olmanız gerekmektedir.";
            }
            else if (ReturnUrl.Contains("AnkaraPage"))
            {
                ViewBag.message = "Bu sayfaya sadece şehir alanı Ankara olan kullanıclar erişebilmektedir.";
            }
            else if(ReturnUrl.Contains("Exchange"))
            {
                ViewBag.message = "30 günlük ücretsiz deneme hakkınız sona ermiştir.";
            }
            else
            {
                ViewBag.message = "Bu sayfaya erişim izininiz bulunmamaktadır.";
            }

            return View();
        }

        [Authorize(Roles = "Manager,Admin")]
        public IActionResult Manager()
        {
            return View();
        }
        
        [Authorize(Roles ="Editör,Admin")]
        public IActionResult Editor()
        {
            return View();
        }


        [Authorize(Policy ="AnkaraPolicy")]
        public IActionResult AnkaraPage()
        {
            return View();
        }
        [Authorize(Policy = "ViolencePolicy")]
        public IActionResult ViolencePage()
        {
            return View();
        }


        public async Task<IActionResult> ExchangeRedirect()
        {

            bool result = User.HasClaim(x => x.Type == "ExpireDateExchange");

            if (!result)
            {
                Claim ExpireDateExchange = new Claim("ExpireDateExchange", DateTime.Now.AddDays(30).Date.ToShortDateString(), ClaimValueTypes.String, "Internal");

                await _userManager.AddClaimAsync(CurrentUser, ExpireDateExchange);
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(CurrentUser, true);
            }

            return RedirectToAction("Exchange");
        }

        [Authorize(Policy = "ExchangePolicy")]
        public IActionResult Exchange()
        {
            return View();
        }
    }
}
