using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

public partial class ChatRoomMember
{
    [Key]
    public int Id { get; set; }

    public int? ChatRoomId { get; set; }

    public int? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? JoinedAt { get; set; }

    [ForeignKey("ChatRoomId")]
    [InverseProperty("ChatRoomMembers")]
    public virtual ChatRoom? ChatRoom { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ChatRoomMembers")]
    public virtual User? User { get; set; }
}
