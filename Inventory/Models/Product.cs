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

    public int? MinQuantity { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? Tax { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? Discount { get; set; }

    [StringLength(10)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "decimal(18, 6)")]
    public decimal Price { get; set; }

    [StringLength(500)]
    public string? Image { get; set; }

    public int CreatedBy { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Products")]
    public virtual Brand Brand { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    [InverseProperty("Products")]
    public virtual Credential CreatedByNavigation { get; set; } = null!;

    [ForeignKey("SubCategoryId")]
    [InverseProperty("Products")]
    public virtual SubCategory SubCategory { get; set; } = null!;

    [ForeignKey("UnitId")]
    [InverseProperty("Products")]
    public virtual Unit Unit { get; set; } = null!;
}
