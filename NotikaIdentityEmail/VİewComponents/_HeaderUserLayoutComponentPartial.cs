using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.VİewComponents
{
    public class _HeaderUserLayoutComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
