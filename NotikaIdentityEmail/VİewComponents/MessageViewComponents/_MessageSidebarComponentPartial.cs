using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.VİewComponents.MessageViewComponents
{
    public class _MessageSidebarComponentPartial : ViewComponent
    {
        private readonly EmailContext _emailContext;
        private readonly UserManager<AppUser> _userManager;
        public _MessageSidebarComponentPartial(EmailContext emailContext, UserManager<AppUser> userManager)
        {
            _emailContext = emailContext;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.SendMessageCount = _emailContext.Messages.Where(x => x.SenderEmail == user.Email).Count();
            ViewBag.InboxMessageCount = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email).Count();


            return View();
        }
    }
}
