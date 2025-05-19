using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using ECommerceMvcSite.Models;
using ECommerceMvcSite.Controllers;

public class MessageController : BaseController
{
    private MyDbContext db = new MyDbContext();

    // Kullanıcı mesaj gönderme sayfası (GET)
    [HttpGet]
    public ActionResult Send()
    {
        return View();
    }

    // Kullanıcı mesaj gönderme (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Send(string name, string email, string content)
    {

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(content))
        {
            ViewBag.Error = "Lütfen tüm alanları doldurun.";
            return View();
        }
        var userId = Session["UserId"] as int?;
        var message = new Message
        {
            UserId = userId,
            UserName = name,
            UserEmail = email,
            Content = content,
            SentAt = DateTime.Now
        };

        db.Messages.Add(message);
        db.SaveChanges();

        ViewBag.Success = "Mesajınız başarıyla gönderildi.";
        return View();
    }

    // Kullanıcının mesajlarını görmesi
    public ActionResult MyMessages()
    {

        var userId = Session["UserId"] as int?;
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var unreadMessages = db.Messages
       .Where(m => m.UserId == userId && !m.IsRead && !string.IsNullOrEmpty(m.AdminResponse))
       .ToList();

        // Mesajları okundu olarak işaretle
        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
        }
        db.SaveChanges();

        var messages = db.Messages.Where(m => m.UserId == userId).OrderByDescending(m => m.SentAt).ToList();
        return View(messages);
    }

    // Admin mesaj listesini görür
    public ActionResult Index()
    {
        var userId = Session["UserId"] as int?;
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var messages = db.Messages.OrderByDescending(m => m.SentAt).ToList();
        return View(messages);
    }

    // Admin cevap verme sayfası (GET)
    public ActionResult Reply(int Id)
    {
        var userId = Session["UserId"] as int?;
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var message = db.Messages.Find(Id);
        if (message == null) return HttpNotFound();

        // Eğer RecipientEmail boşsa, UserEmail'den ayarla
        if (string.IsNullOrEmpty(message.RecipientEmail) && !string.IsNullOrEmpty(message.UserEmail))
        {
            message.RecipientEmail = message.UserEmail;
        }

        // Debug log
        System.Diagnostics.Debug.WriteLine("Reply GET - Loaded Message:");
        System.Diagnostics.Debug.WriteLine("  Id: " + message.Id);
        System.Diagnostics.Debug.WriteLine("  UserEmail: " + message.UserEmail);
        System.Diagnostics.Debug.WriteLine("  RecipientEmail: " + message.RecipientEmail);
        System.Diagnostics.Debug.WriteLine("  UserId: " + message.UserId);

        return View(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Reply(int Id, string AdminResponse, string RecipientEmail, int? UserId)
    {
        var adminId = Session["UserId"] as int?;
        var isAdmin = Session["IsAdmin"] as bool? ?? false;

        if (adminId == null || !isAdmin)
        {
            return RedirectToAction("Login", "Account");
        }
        System.Diagnostics.Debug.WriteLine($"🔍 Admin yanıt veriyor - ID: {Id}, AdminResponse: {AdminResponse}");
        System.Diagnostics.Debug.WriteLine($"📬 Gelen RecipientEmail: {RecipientEmail}");
        System.Diagnostics.Debug.WriteLine($"👤 Gelen UserId: {UserId}");
        var message = db.Messages.Find(Id);
        if (message == null)
        {
            return HttpNotFound();
        }

        // Güncellenen alanlar:
        message.AdminResponse = AdminResponse;
        message.ResponseDate = DateTime.Now;

        if (!string.IsNullOrEmpty(RecipientEmail))
            message.RecipientEmail = RecipientEmail;

        if (UserId.HasValue)
            message.UserId = UserId.Value;
        message.IsRead = false;
        db.SaveChanges();
        try
        {
            var fromAddress = new MailAddress("masakioyuncak@gmail.com", "Masakı Oyuncak");
            var toAddress = new MailAddress(RecipientEmail);
            const string fromPassword = "fwpx nvmj caja anop"; // ✅ Gmail uygulama şifresi
            const string subject = "Mesajınıza Yanıt Geldi";
            string body = $"Merhaba,\n\nMesajınıza gelen yanıt:\n\n{AdminResponse}\n\nMasakı Oyuncak İyi Günler Diler!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(mailMessage);
            }

            TempData["Success"] = "Cevap başarıyla gönderildi ve mail yollandı.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Mail gönderilemedi: " + ex.Message;
        }

        return RedirectToAction("Index");
    }
 

    public ActionResult Messages()
    {
        if (Session["UserRole"] == null)
            return RedirectToAction("Login", "Account");

        string role = Session["UserRole"].ToString();
        List<Message> messages;

        if (role == "Admin")
        {
            messages = db.Messages.ToList(); // Admin tüm mesajları görür
        }
        else
        {
            var userEmail = Session["UserEmail"]?.ToString();
            messages = db.Messages.Where(m => m.RecipientEmail == userEmail).ToList();
        }

        return View(messages); // Views/Message/Messages.cshtml dosyasına gider
    }
    public ActionResult Details(int Id)
    {
        var message = db.Messages.Find(Id);
        if (message == null)
            return HttpNotFound();
        if (message != null && !message.IsRead)
        {
            message.IsRead = true;  // artık kullanıcı mesajı okudu
            db.SaveChanges();
        }
        return View(message);
    }

}
