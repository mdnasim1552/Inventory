using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("StockTransfer")]
public partial class StockTransfer
{
    [Key]
    public int Id { get; set; }

    public int? FromStoreId { get; set; }

    public int ToStoreId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime TransferDate { get; set; }

    [InverseProperty("StockTransfer")]
    public virtual ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();
}
