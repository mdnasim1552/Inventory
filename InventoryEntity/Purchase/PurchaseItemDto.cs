using InventoryEntity.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InventoryEntity.Purchase
{
    public class PurchaseItemDto
    {
        public int Id { get; set; }

        public int PurchaseId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal UnitCost { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Tax { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Total { get; set; }
        public ProductDto Product { get; set; } = null!;
    }
}
