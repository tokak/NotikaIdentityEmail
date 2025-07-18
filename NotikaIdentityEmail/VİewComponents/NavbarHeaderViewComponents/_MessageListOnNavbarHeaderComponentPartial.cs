using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.MessageViewModels;

namespace NotikaIdentityEmail.VİewComponents.NavbarHeaderViewComponents
{
    public class _MessageListOnNavbarHeaderComponentPartial : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _emailContext;

        public _MessageListOnNavbarHeaderComponentPartial(UserManager<AppUser> userManager, EmailContext emailContext)
        {
            _userManager = userManager;
            _emailContext = emailContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var values = (from m in _emailContext.Messages
                          join u in _emailContext.Users
                          on m.SenderEmail equals u.Email into userGroup
                          from u in userGroup.DefaultIfEmpty()
                          where m.ReceiverEmail == user.Email && m.IsRead == false
                          select new MessageListWithUserInfo
                          {
                              FullName = u != null ? u.Name + " " + u.Surname : m.SenderEmail,
                              ImageUrl = u != null ? u.ImageUrl : null,
                              MessageDetail = m.Subject,
                              SendDate = m.SendDate
                          }).ToList();

            return View(values);

        }
    }
}
