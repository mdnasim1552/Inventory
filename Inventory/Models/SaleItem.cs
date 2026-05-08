using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("SaleItem")]
public partial class SaleItem
{
    [Key]
    public int Id { get; set; }

    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? Tax { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? Total { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("SaleItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("SaleId")]
    [InverseProperty("SaleItems")]
    public virtual Sale Sale { get; set; } = null!;
}
