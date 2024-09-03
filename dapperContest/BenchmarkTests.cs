using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using dapperContest.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 5)]
[MemoryDiagnoser]
[Config(typeof(Config))]
public class BenchmarkTests
{
    private class Config : ManualConfig
    {
        public Config()
        {
            AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Default);
            AddColumnProvider(BenchmarkDotNet.Columns.DefaultColumnProviders.Instance);
            AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
        }
    }

    private readonly string _connectionString = "Data Source=benchmark.db";
    private readonly List<Person> _people;
    private readonly DapperService _dapperService;
    private readonly DapperPlusService _dapperPlusService;
    private const int OperationsPerIteration = 5; // Increase this number to get longer iteration times

    public BenchmarkTests()
    {
        _people = Enumerable.Range(1, 10000).Select(i => new Person
        {
            Name = $"Person {i}",
            Age = 20 + (i % 50),
            Email = $"person{i}@example.com"
        }).ToList();

        _dapperService = new DapperService(_connectionString);
        _dapperPlusService = new DapperPlusService(_connectionString);

        SetupDatabase();
    }

    private void SetupDatabase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS People (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Age INTEGER,
                        Email TEXT
                    )";
                command.ExecuteNonQuery();
            }
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM People";
                command.ExecuteNonQuery();
            }
        }
    }

    [Benchmark]
    public void DapperBulkInsert()
    {
        for (int i = 0; i < OperationsPerIteration; i++)
        {
            _dapperService.BulkInsert(_people);
        }
    }

    [Benchmark]
    public void DapperPlusBulkInsert()
    {
        for (int i = 0; i < OperationsPerIteration; i++)
        {
            _dapperPlusService.BulkInsert(_people);
        }
    }

    [Benchmark]
    public void AdoNetSqliteRawBulkInsert()
    {
        for (int i = 0; i < OperationsPerIteration; i++)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO People (Name, Age, Email) VALUES (@Name, @Age, @Email)";
                        var nameParam = command.CreateParameter();
                        var ageParam = command.CreateParameter();
                        var emailParam = command.CreateParameter();

                        nameParam.ParameterName = "@Name";
                        ageParam.ParameterName = "@Age";
                        emailParam.ParameterName = "@Email";

                        command.Parameters.Add(nameParam);
                        command.Parameters.Add(ageParam);
                        command.Parameters.Add(emailParam);

                        foreach (var person in _people)
                        {
                            nameParam.Value = person.Name;
                            ageParam.Value = person.Age;
                            emailParam.Value = person.Email;
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }
    }

}