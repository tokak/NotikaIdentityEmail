using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class ErrorPageController : Controller
    {
        [Route("ErrorPage/Page404")]
        public IActionResult Page404()
        {
            return View();
        }

        [Route("ErrorPage/Page401")]
        public IActionResult Page401()
        {
            return View();
        }

        [Route("ErrorPage/Page403")]
        public IActionResult Page403()
        {
            return View();
        }

        [Route("ErrorPage/HandleError")]
        public IActionResult HandleError(int statusCode)
        {
            if (statusCode == 404)
            {
                return RedirectToAction("Page404");
            }
            else if (statusCode == 401)
            {
                return RedirectToAction("Page401");
            }
            else if (statusCode == 403)
            {
                return RedirectToAction("Page403");
            }
            // Genel hata görünümü
            return View("GenericError", statusCode);
        }
    }

}
