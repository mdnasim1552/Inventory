using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("Category")]
public partial class Category
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    [StringLength(500)]
    public string? Image { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("Category")]
    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
