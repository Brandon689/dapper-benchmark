using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using DapperBulk.Mapping;
using DapperBulk.Utilities;
using Dapper;

namespace DapperBulk.BulkOperations
{
    internal class BulkInsert<T>
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly int? _commandTimeout;

        public BulkInsert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout)
        {
            _connection = connection;
            _transaction = transaction;
            _commandTimeout = commandTimeout;
        }

        public int Execute(IEnumerable<T> entities, string tableName = null, int? batchSize = null)
        {
            if (entities == null || !entities.Any())
                return 0;

            var entityType = typeof(T);
            var tableInfo = EntityMapper.GetTableInfo(entityType);
            tableName = tableName ?? tableInfo.TableName;
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table name must be provided either through the TableAttribute or as a parameter.");
            }
            var properties = PropertyMapper.GetInsertableProperties(entityType);

            return BulkInsertToSqlite(entities, tableName, properties, batchSize ?? DapperBulkConfiguration.DefaultBatchSize);
        }


        private int BulkInsertToSqlite(IEnumerable<T> entities, string tableName, IEnumerable<System.Reflection.PropertyInfo> properties, int batchSize)
        {
            var propertyNames = properties.Where(p => p.Name != "Id").Select(p => p.Name).ToList();
            var sqlBuilder = new StringBuilder($"INSERT INTO {tableName} ({string.Join(", ", propertyNames)}) VALUES ");
            var valuesSql = string.Join(", ", Enumerable.Repeat("?", propertyNames.Count));

            int totalInserted = 0;

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    using (var cmd = _connection.CreateCommand())
                    {
                        cmd.CommandText = $"{sqlBuilder}({valuesSql})";
                        cmd.Transaction = transaction;

                        foreach (var prop in propertyNames)
                        {
                            cmd.Parameters.Add(new SqliteParameter());
                        }

                        foreach (var entity in entities)
                        {
                            for (int i = 0; i < propertyNames.Count; i++)
                            {
                                cmd.Parameters[i].Value = properties.ElementAt(i).GetValue(entity) ?? DBNull.Value;
                            }
                            totalInserted += cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during bulk insert: {ex.Message}");
                    transaction.Rollback();
                    throw;
                }
            }

            return totalInserted;
        }




    }
}
