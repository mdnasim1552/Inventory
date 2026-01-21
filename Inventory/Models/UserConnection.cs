using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

public partial class UserConnection
{
    [Key]
    [StringLength(200)]
    public string ConnectionId { get; set; } = null!;

    public int? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ConnectedAt { get; set; }
}
