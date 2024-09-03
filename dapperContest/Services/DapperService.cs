using Dapper;
using dapperContest.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data;

public class DapperService
{
    private readonly string _connectionString;

    public DapperService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void BulkInsert(IEnumerable<Person> people)
    {
        using (IDbConnection db = new SqliteConnection(_connectionString))
        {
            db.Open();
            using (var transaction = db.BeginTransaction())
            {
                foreach (var person in people)
                {
                    db.Execute("INSERT INTO People (Name, Age, Email) VALUES (@Name, @Age, @Email)", person, transaction);
                }
                transaction.Commit();
            }
        }
    }
}
