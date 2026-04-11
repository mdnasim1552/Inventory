using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("PurchaseItem")]
public partial class PurchaseItem
{
    [Key]
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

    [Column(TypeName = "datetime")]
    public DateTime ExpiryDate { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("PurchaseItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("PurchaseId")]
    [InverseProperty("PurchaseItems")]
    public virtual Purchase Purchase { get; set; } = null!;
}
