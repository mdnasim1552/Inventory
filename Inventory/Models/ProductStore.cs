using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("ProductStore")]
public partial class ProductStore
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int StoreId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductStores")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("StoreId")]
    [InverseProperty("ProductStores")]
    public virtual Store Store { get; set; } = null!;
}
