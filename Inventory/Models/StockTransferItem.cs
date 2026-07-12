using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Models;

[Table("StockTransferItem")]
[Index("ProductId", Name = "IX_StockTransferItem_ProductId")]
[Index("StockTransferId", Name = "IX_StockTransferItem_StockTransferId")]
public partial class StockTransferItem
{
    [Key]
    public int Id { get; set; }

    public int StockTransferId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("StockTransferItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("StockTransferId")]
    [InverseProperty("StockTransferItems")]
    public virtual StockTransfer StockTransfer { get; set; } = null!;
}
