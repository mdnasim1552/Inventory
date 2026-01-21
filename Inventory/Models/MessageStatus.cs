using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("MessageStatus")]
public partial class MessageStatus
{
    [Key]
    public long Id { get; set; }

    public long? MessageId { get; set; }

    public int? UserId { get; set; }

    public bool? IsSeen { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? SeenAt { get; set; }

    [ForeignKey("MessageId")]
    [InverseProperty("MessageStatuses")]
    public virtual Message? Message { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("MessageStatuses")]
    public virtual User? User { get; set; }
}
