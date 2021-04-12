namespace TenderManagement.Database
{
    public class ConsoleTraceService : DefaultTraceService
    {
        protected override void Log(string prefix, string message) => System.Console.WriteLine($"{prefix}: {message}");
    }
}
