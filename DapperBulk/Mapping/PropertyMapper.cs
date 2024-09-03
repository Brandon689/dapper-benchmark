using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace DapperBulk.Mapping
{
    internal static class PropertyMapper
    {
        public static IEnumerable<PropertyInfo> GetInsertableProperties(Type type)
        {
            return type.GetProperties()
                .Where(p => p.Name != "Id" &&
                            p.GetCustomAttribute<NotMappedAttribute>() == null &&
                            p.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption != DatabaseGeneratedOption.Identity);
        }
    }

}
