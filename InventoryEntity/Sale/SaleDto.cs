using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text;

namespace InventoryEntity.Sale
{
    public class SaleDto
    {
        public int Id { get; set; }

        public int StoreId { get; set; }

        public int? CustomerId { get; set; }

        [StringLength(100)]
        public string? InvoiceNo { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? SaleDate { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? SubTotal { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Tax { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? TotalAmount { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? PaidAmount { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal? DueAmount { get; set; }

        public int CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
    }
}
