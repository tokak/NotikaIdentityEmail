using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailContext _context;

        public LoginController(SignInManager<AppUser> signInManager, EmailContext context, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginUserViewModel model)
        {
            var value = _context.Users.Where(x=>x.UserName == model.Username).FirstOrDefault();
            if (value == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı bulunamadı.");
                return View();
            }
            bool isPasswordValid = await _userManager.CheckPasswordAsync(value, model.Password);
            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifreniz hatalı");
                return View();
            }

            if (value.EmailConfirmed)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("EditProfile", "Profile");
                }
            }
            
            TempData["EmailMove"] = value.Email;
            return RedirectToAction("UserActivation", "Activation");



        }
    }
}
