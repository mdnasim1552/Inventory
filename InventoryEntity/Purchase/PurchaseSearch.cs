using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryEntity.Purchase
{
    public class PurchaseSearch
    {
        public string? SupplierName { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? PurchaseDateFrom { get; set; }
        public DateTime? PurchaseDateTo { get; set; }
    }
}
