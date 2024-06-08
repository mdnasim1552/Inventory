using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

public partial class PasswordResetToken
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [StringLength(50)]
    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    [Column(TypeName = "smalldatetime")]
    public DateTime Expiration { get; set; }
}
