using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;

namespace NotikaIdentityEmail.VİewComponents.NavbarHeaderViewComponents
{
    public class _NotificationListOnNavbarComponentPartial : ViewComponent
    {
        private readonly EmailContext _emailContext;

        public _NotificationListOnNavbarComponentPartial(EmailContext emailContext)
        {
            _emailContext = emailContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = _emailContext.Notifications.ToList();
            return View(values);
        }
    }
}
