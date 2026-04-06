using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("Purchase")]
public partial class Purchase
{
    [Key]
    public int Id { get; set; }

    public int SupplierId { get; set; }

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

    [InverseProperty("Purchase")]
    public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    [ForeignKey("SupplierId")]
    [InverseProperty("Purchases")]
    public virtual Supplier Supplier { get; set; } = null!;
}
