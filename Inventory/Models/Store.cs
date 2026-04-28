using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("Store")]
public partial class Store
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Code { get; set; } = null!;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Store")]
    public virtual ICollection<Credential> Credentials { get; set; } = new List<Credential>();

    [InverseProperty("Store")]
    public virtual ICollection<ProductStore> ProductStores { get; set; } = new List<ProductStore>();

    [InverseProperty("Store")]
    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
