using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Extensions
{
    public static class DataTableExtensions
    {
        public static List<T> DataTableToList<T>(this DataTable dataTable) where T : new()//without extension public static List<T> DataTableToList<T>(DataTable dataTable) where T : new()
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable), "The input DataTable is null.");
            }
            List<T> dataList = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                T rowData = new T();

                foreach (DataColumn col in dataTable.Columns)
                {
                    PropertyInfo property = typeof(T).GetProperty(col.ColumnName);
                    if (property != null && row[col] != DBNull.Value)
                    {
                        property.SetValue(rowData, Convert.ChangeType(row[col], property.PropertyType));
                    }
                }

                dataList.Add(rowData);
            }

            return dataList;
        }
    }
}
