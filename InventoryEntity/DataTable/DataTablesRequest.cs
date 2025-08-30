using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryEntity.DataTable
{
    public class DataTablesRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }   // starting index
        public int Length { get; set; }  // page size
        //public string Search { get; set; }
        public SearchInfo Search { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        public List<OrderInfo> Order { get; set; }
    }
}
