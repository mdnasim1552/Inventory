using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryEntity.Product
{
    public class ProductSearch
    {
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? Min_Price { get; set;}
        public int? Max_Price { get;set;}
    }
}
