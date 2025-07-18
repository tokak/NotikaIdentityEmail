using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.VİewComponents
{
    public class _HeaderUserLayoutComponentPartial:ViewComponent
    {
        private readonly EmailContext _emailContext;
        private readonly UserManager<AppUser> _userManager;
        public _HeaderUserLayoutComponentPartial(EmailContext emailContext, UserManager<AppUser> userManager)
        {
            _emailContext = emailContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            string currentUserEmail = currentUser.Email;

            var userEmailCount = (from m in _emailContext.Messages
                                  where m.ReceiverEmail == currentUserEmail && m.IsRead == false
                                  select m).Count();
            ViewBag.userEmailCount = userEmailCount;
            ViewBag.notificationCount = (from n in _emailContext.Notifications
                                         select n).Count();
            return View();
        }
    }
}
