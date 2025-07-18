using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.MessageViewModels;

namespace NotikaIdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;
        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            // Giriş yapan kullanıcının e-posta adresine göre filtreleme yapılır
            //var values = (from m in _context.Messages
            //              join u in _context.Users
            //              on m.SenderEmail equals u.Email // Gönderen kullanıcıyı eşleştiriyoruz
            //              where m.ReceiverEmail == user.Email // Sadece giriş yapan kullanıcıya gelen mesajlar
            //              select new MessageWithSenderInfoViewModel
            //              {
            //                  MessageId = m.Id,
            //                  MessageDetail = m.MessageDetail,
            //                  SendDate = m.SendDate,
            //                  SenderEmail = m.SenderEmail,
            //                  Subject = m.Subject,
            //                  SenderName = u.Name,
            //                  SenderSurname = u.Surname
            //              }).ToList();

            // Giriş yapan kullanıcının e-posta adresine göre filtreleme yapılır
            var values = (from m in _context.Messages
                          join u in _context.Users
                          on m.SenderEmail equals u.Email into userGroup
                          from sender in userGroup.DefaultIfEmpty()
                          join c in _context.Categories
                          on m.CategoryId equals c.Id into categoryGroup
                          from category in categoryGroup.DefaultIfEmpty()
                          where m.ReceiverEmail == user.Email // Sadece giriş yapan kullanıcıya gelen mesajlar
                          select new MessageWithSenderInfoViewModel
                          {
                              MessageId = m.Id,
                              MessageDetail = m.MessageDetail,
                              SendDate = m.SendDate,
                              SenderEmail = m.SenderEmail,
                              Subject = m.Subject,
                              SenderName = sender != null ? sender.Name : "Bilinmeyen",
                              SenderSurname = sender != null ? sender.Surname : "Kullanıcı",
                              CategoryName =  category != null ? category.Name : "Kategori Yok",
                          }).ToList();
            return View(values);
        }

        public async Task<IActionResult> Sendbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var values = (from m in _context.Messages
                          join u in _context.Users
                          on m.ReceiverEmail equals u.Email into userGroup
                          from receiver in userGroup.DefaultIfEmpty()
                          join c in _context.Categories
                          on m.CategoryId equals c.Id into categoryGroup
                          from category in categoryGroup.DefaultIfEmpty()
                          where m.SenderEmail == user.Email // Sadece giriş yapan kullanıcıya gelen mesajlar
                          select new MessageWithReceiverInfoViewModel
                          {
                              MessageId = m.Id,
                              MessageDetail = m.MessageDetail,
                              SendDate = m.SendDate,
                              ReceiverEmail = m.ReceiverEmail,
                              Subject = m.Subject,
                              ReceiverName = receiver != null ? receiver.Name : "Bilinmeyen",
                              ReceiverSurname = receiver != null ? receiver.Surname : "Kullanıcı",
                              CategoryName = category != null ? category.Name : "Kategori Yok",
                          }).ToList();
            return View(values);
        }

        public IActionResult MessageDetail(int id)
        {
            var values = _context.Messages.Where(x => x.Id == 1).FirstOrDefault();
            return View(values);
        }
        [HttpGet]
        public IActionResult ComposeMessage()
        {
            var categories = _context.Categories.ToList();
            ViewBag.categories = categories.Select(x=> new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ComposeMessage(Message message)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            message.SenderEmail = user.Email;
            message.ReceiverEmail = message.ReceiverEmail;
            message.SendDate = DateTime.Now;
            message.IsRead = false;
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Sendbox");
        }

        public async Task<IActionResult> GetMessageListByCategory(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var values = (from m in _context.Messages
                          join u in _context.Users
                          on m.SenderEmail equals u.Email into userGroup
                          from sender in userGroup.DefaultIfEmpty()

                          join c in _context.Categories
                          on m.CategoryId equals c.Id into categoryGroup
                          from category in categoryGroup.DefaultIfEmpty()

                          where m.ReceiverEmail == user.Email && m.CategoryId == id
                          select new MessageWithSenderInfoViewModel
                          {
                              MessageId = m.Id,
                              MessageDetail = m.MessageDetail,
                              Subject = m.Subject,
                              SendDate = m.SendDate,
                              SenderEmail = m.SenderEmail,
                              SenderName = sender != null ? sender.Name : "Bilinmeyen",
                              SenderSurname = sender != null ? sender.Surname : "Kullanıcı",
                              CategoryName = category != null ? category.Name : "Kategori Yok"
                          }).ToList();

            return View(values);
        }
    }
}
