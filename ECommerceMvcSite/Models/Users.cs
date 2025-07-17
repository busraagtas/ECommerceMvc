using System.ComponentModel.DataAnnotations;

namespace ECommerceMvcSite.Models
{
    public class User
    {
        public int Id { get; set; }

        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [Display(Name = "E-posta")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

    }
}
