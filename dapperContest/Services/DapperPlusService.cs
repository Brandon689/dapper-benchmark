using dapperContest.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Z.Dapper.Plus;

public class DapperPlusService
{
    private readonly string _connectionString;

    public DapperPlusService(string connectionString)
    {
        _connectionString = connectionString;
        // Configure the mapper for the Person class
        DapperPlusManager.Entity<Person>().Table("People");
    }

    public void BulkInsert(IEnumerable<Person> people)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            connection.BulkInsert(people);
        }
    }
}
