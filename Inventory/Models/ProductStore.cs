using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("ProductStore")]
[Index("ProductId", Name = "IX_ProductStore_ProductId")]
[Index("StoreId", Name = "IX_ProductStore_StoreId")]
public partial class ProductStore
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int StoreId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal SellingPrice { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductStores")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("StoreId")]
    [InverseProperty("ProductStores")]
    public virtual Store Store { get; set; } = null!;
}
