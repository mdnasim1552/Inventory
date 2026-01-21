using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

public partial class Message
{
    [Key]
    public long MessageId { get; set; }

    public int? ChatRoomId { get; set; }

    public int? SenderId { get; set; }

    public string? MessageText { get; set; }

    [StringLength(20)]
    public string? MessageType { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("ChatRoomId")]
    [InverseProperty("Messages")]
    public virtual ChatRoom? ChatRoom { get; set; }

    [InverseProperty("Message")]
    public virtual ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();

    [ForeignKey("SenderId")]
    [InverseProperty("Messages")]
    public virtual User? Sender { get; set; }
}
