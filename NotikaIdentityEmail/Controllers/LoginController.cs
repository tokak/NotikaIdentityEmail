using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailContext _context;

        public LoginController(SignInManager<AppUser> signInManager, EmailContext context)
        {
            _signInManager = signInManager;
            _context = context;
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
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifreniz hatalı");
                return View();
            }
            if (value.EmailConfirmed)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("EditProfile","Profile");
                }
            }
            TempData["EmailMove"] = value.Email;
            return RedirectToAction("UserActivation", "Activation");



        }
    }
}
