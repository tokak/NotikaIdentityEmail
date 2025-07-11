﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        public LoginController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginUserViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username,model.Password,true,true);
            if (result.Succeeded)
            {
                return RedirectToAction("Profile","MyProfile");
            }
            ModelState.AddModelError(string.Empty,"Kullanıcı adı veya şifreniz hatalı");
            return View();
        }
    }
}
