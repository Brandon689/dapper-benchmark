namespace DapperBulk
{
    public static class DapperBulkConfiguration
    {
        public static int DefaultBatchSize { get; set; } = 1000;
        public static int DefaultCommandTimeout { get; set; } = 30;
    }
}
