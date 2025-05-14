using System.ComponentModel.DataAnnotations;

namespace ECommerceMvcSite.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Fiyat alanı zorunludur.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        [Display(Name = "Stok")]
       // [Required(ErrorMessage = "Stok bilgisi zorunludur.")]
        public int Stock { get; set; }

        [Display(Name = "Görsel Yolu")]
        public string ImageUrl { get; set; }
    }
}
