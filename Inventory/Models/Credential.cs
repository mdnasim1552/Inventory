using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("Credential")]
public partial class Credential
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string? Gender { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime? Birthday { get; set; }

    public int RoleId { get; set; }

    [StringLength(500)]
    public string? Image { get; set; }

    [StringLength(15)]
    public string? Mobile { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime? CreatedOn { get; set; }

    public int? ParentId { get; set; }

    [StringLength(100)]
    public string? Designation { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Credentials")]
    public virtual Userrole Role { get; set; } = null!;
}
