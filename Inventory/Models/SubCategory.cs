using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("SubCategory")]
public partial class SubCategory
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("SubCategories")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("SubCategory")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
