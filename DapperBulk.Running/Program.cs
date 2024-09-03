using BenchmarkDotNet.Running;
using DapperBulk.Tests;

class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<DapperBulkBenchmarks>();
    }
}
