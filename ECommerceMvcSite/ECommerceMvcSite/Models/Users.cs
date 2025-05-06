using System.ComponentModel.DataAnnotations;

namespace ECommerceMvcSite.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsAdmin { get; set; } = false;

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
