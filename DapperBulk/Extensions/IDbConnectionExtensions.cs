using System.Collections.Generic;
using System.Data;
using DapperBulk.BulkOperations;

namespace DapperBulk.Extensions
{
    public static class IDbConnectionExtensions
    {
        public static int BulkInsert<T>(this IDbConnection connection, IEnumerable<T> entities, string tableName = null, IDbTransaction transaction = null, int? batchSize = null, int? commandTimeout = null)
        {
            var bulkInsert = new BulkInsert<T>(connection, transaction, commandTimeout);
            return bulkInsert.Execute(entities, tableName, batchSize);
        }
    }
}
