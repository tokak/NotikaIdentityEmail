using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;

namespace NotikaIdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;

        public MessageController(EmailContext context)
        {
            _context = context;
        }

        public IActionResult Inbox()
        {
            var values = _context.Messages.Where(x=>x.ReceiverEmail == "tokakmurat01@gmail.com").ToList();
            return View(values);
        }
        
        public IActionResult Sendbox()
        {
            var values = _context.Messages.Where(x => x.SenderEmail == "ferhatcengiz@gmail.com").ToList();
            return View(values);
        }

        public IActionResult MessageDetail()
        {
            var values = _context.Messages.Where(x => x.Id == 1).FirstOrDefault();
            return View(values);
        }
        [HttpGet]
        public IActionResult ComposeMessage()
        {
            var values = _context.Messages.Where(x => x.Id == 1).FirstOrDefault();
            return View(values);
        }
    }
}
