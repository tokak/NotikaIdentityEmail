using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            UserEditViewModel userEditViewModel = new();
            userEditViewModel.Name = values.Name;
            userEditViewModel.Surname = values.Surname;
            userEditViewModel.Phonenumber = values.PhoneNumber;
            userEditViewModel.City = values.City;
            userEditViewModel.ImageUrl = values.ImageUrl;
            userEditViewModel.Email = values.Email;
            return View(userEditViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditViewModel model)
        {
            if (model.Password == model.PasswordConfirm)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                user.Name = model.Name;
                user.Surname = model.Surname;
                user.PhoneNumber = model.Phonenumber;
                user.City = model.City;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.ImageUrl = model.ImageUrl;
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                _userManager.UpdateAsync(user);
            }
            return View();
        }
    }
}