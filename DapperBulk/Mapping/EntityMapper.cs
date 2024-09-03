using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace DapperBulk.Mapping
{
    internal static class EntityMapper
    {
        public static TableInfo GetTableInfo(Type entityType)
        {
            var tableName = entityType.Name;
            var tableAttr = entityType.GetCustomAttribute<TableAttribute>();
            if (tableAttr != null)
            {
                tableName = tableAttr.Name;
            }

            return new TableInfo { TableName = tableName };
        }
    }

    internal class TableInfo
    {
        public string TableName { get; set; }
    }
}
