using System;
using System.Collections.Generic;
using System.Data;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Dapper;
using DapperBulk.Extensions;
using Microsoft.Data.Sqlite;
using Xunit;

namespace DapperBulk.Tests
{
    public class Person
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }


    [MemoryDiagnoser]
    public class DapperBulkBenchmarks
    {
        private const string ConnectionString = "Data Source=:memory:";
        private readonly List<Person> _people;
        private const int NumberOfRecords = 10000;
        private SqliteConnection _connection;

        public DapperBulkBenchmarks()
        {
            _people = new List<Person>();
            for (int i = 0; i < NumberOfRecords; i++)
            {
                _people.Add(new Person
                {
                    Name = $"Person {i}",
                    Age = 20 + (i % 50),
                    Email = $"person{i}@example.com"
                });
            }
        }


        private void SetupDatabase(IDbConnection connection)
        {
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS People (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Age INTEGER,
                    Email TEXT
                )");
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();
            SetupDatabase(_connection);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _connection?.Dispose();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _connection.Execute("DELETE FROM People");
        }

        [Benchmark(Baseline = true)]
        public void StandardDapperInsert()
        {
            foreach (var person in _people)
            {
                _connection.Execute("INSERT INTO People (Name, Age, Email) VALUES (@Name, @Age, @Email)", person);
            }
        }


        [Benchmark]
        public void DapperBulkInsert()
        {
            _connection.BulkInsert(_people, "People");
        }
    }

    public class BenchmarkTests
    {
        [Fact]
        public void RunBenchmarks()
        {
            var summary = BenchmarkRunner.Run<DapperBulkBenchmarks>();
        }
    }
}
