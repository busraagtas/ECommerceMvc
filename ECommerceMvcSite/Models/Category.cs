//using Microsoft.Analytics.Interfaces;
//using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ECommerceMvcSite.Models
{
   public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Her kategori birden çok ürüne sahip olabilir
        public virtual ICollection<Product> Products { get; set; }
    }
}