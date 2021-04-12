namespace TenderManagement.Database
{
    public class DebugTraceService : DefaultTraceService
    {
        protected override void Log(string prefix, string message) =>
            System.Diagnostics.Debug.WriteLine($"{prefix}: {message}");
    }
}
