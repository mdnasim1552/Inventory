using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }

    public int BrandId { get; set; }

    public int UnitId { get; set; }

    [Column("SKU")]
    [StringLength(50)]
    public string? Sku { get; set; }

    public string? Description { get; set; }

    [StringLength(500)]
    public string? Image { get; set; }

    public int CreatedBy { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Products")]
    public virtual Brand Brand { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    [InverseProperty("Products")]
    public virtual Credential CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ProductStore> ProductStores { get; set; } = new List<ProductStore>();

    [InverseProperty("Product")]
    public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    [InverseProperty("Product")]
    public virtual ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();

    [ForeignKey("SubCategoryId")]
    [InverseProperty("Products")]
    public virtual SubCategory SubCategory { get; set; } = null!;

    [ForeignKey("UnitId")]
    [InverseProperty("Products")]
    public virtual Unit Unit { get; set; } = null!;
}
