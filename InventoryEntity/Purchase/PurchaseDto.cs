using InventoryEntity.Product;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InventoryEntity.Purchase
{
    public class PurchaseDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int SupplierId { get; set; }
        [StringLength(100)]
        public string? SupplierName { get; set; }
        [StringLength(100)]
        public string? InvoiceNo { get; set; }
        [StringLength(500)]
        public string? InvoiceFile { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime PurchaseDate { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? SubTotal { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Tax { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? TotalAmount { get; set; }

        public int? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        public IFormFile? Invoice_File { get; set; }
        public PurchaseItemDto? PurchaseItem { get; set; }
        public string? PurchaseItemsJson { get; set; }
    }
}
