using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DapperBulk.Utilities
{
    internal static class DataTableExtensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> data, IEnumerable<PropertyInfo> properties)
        {
            var dataTable = new DataTable();

            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            foreach (var item in data)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
