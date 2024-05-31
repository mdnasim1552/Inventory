using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("Userrole")]
public partial class Userrole
{
    [Key]
    public int RoleId { get; set; }

    [StringLength(100)]
    public string Role { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<Credential> Credentials { get; set; } = new List<Credential>();
}
