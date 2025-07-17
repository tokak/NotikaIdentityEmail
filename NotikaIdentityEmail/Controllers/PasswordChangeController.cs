using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.ForgetPasswordModel;

namespace NotikaIdentityEmail.Controllers
{
    public class PasswordChangeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public PasswordChangeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModels model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Kullanıcı bulunamazsa hata mesajını ViewBag ile gönderelim
                ViewBag.Error = "Kullanıcı bulunamadı";
                return View(model);
            }

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetTokenLink = Url.Action("ResetPassword", "PasswordChange", new
            {
                userId = user.Id,
                token = passwordResetToken
            }, HttpContext.Request.Scheme);

            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin", "murattokak.21@gmail.com");
            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User", model.Email);
            mimeMessage.To.Add(mailboxAddressTo);

            var bodyBuilder = new BodyBuilder();
            // HTML formatında link yollamak için TextBody değil HtmlBody kullanalım
            bodyBuilder.HtmlBody = $"Şifre yenileme linki için <a href=\"{passwordResetTokenLink}\">burayı tıklayınız</a>.";
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            mimeMessage.Subject = "Şifre Değişiklik Talebi";
            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("murattokak.21@gmail.com", "lecsplerevwjjmsn");
            client.Send(mimeMessage);
            client.Disconnect(true);

            // Başarı mesajını ViewBag ile gönderelim
            ViewBag.Message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
            return View();
        }


        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var userid = TempData["userId"].ToString();
            var token = TempData["token"].ToString();
            
            if (userid == null || token == null)
            {
                ModelState.AddModelError(string.Empty, "Mail veya kullanıcı bulunamadı.");
                return View();

            }
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.Error = "Kullanıcı bulunamadı";
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("UserLogin","Login");
            }
            else
            {

                    ModelState.AddModelError(string.Empty, "Mail veya kullanıcı bulunamadı.");
             
                return View();
            }
        }

    }
}
