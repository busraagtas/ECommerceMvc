//using Microsoft.Analytics.Interfaces;
//using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ECommerceMvcSite.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // Mesajı gönderen kullanıcı (giriş yapmışsa)
        public string UserName { get; set; } // Giriş yapılmamışsa isim burada saklanabilir
        public string UserEmail { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public string AdminResponse { get; set; } // Adminin cevabı
        public DateTime? ResponseDate { get; set; }
        public string RecipientEmail { get; set; }
    }
}