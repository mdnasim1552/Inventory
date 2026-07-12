using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("Sale")]
[Index("CreatedBy", Name = "IX_Sale_CreatedBy")]
[Index("CustomerId", Name = "IX_Sale_CustomerId")]
[Index("StoreId", Name = "IX_Sale_StoreId")]
public partial class Sale
{
    [Key]
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

    [ForeignKey("CreatedBy")]
    [InverseProperty("Sales")]
    public virtual Credential CreatedByNavigation { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("Sales")]
    public virtual Customer? Customer { get; set; }

    [InverseProperty("Sale")]
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    [ForeignKey("StoreId")]
    [InverseProperty("Sales")]
    public virtual Store Store { get; set; } = null!;
}
