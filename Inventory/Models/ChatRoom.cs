using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

public partial class ChatRoom
{
    [Key]
    public int ChatRoomId { get; set; }

    [StringLength(100)]
    public string? RoomName { get; set; }

    public bool IsGroup { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("ChatRoom")]
    public virtual ICollection<ChatRoomMember> ChatRoomMembers { get; set; } = new List<ChatRoomMember>();

    [InverseProperty("ChatRoom")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
