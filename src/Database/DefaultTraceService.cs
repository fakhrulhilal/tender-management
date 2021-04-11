namespace TenderManagement.Database
{
    public class DefaultTraceService : Yuniql.Extensibility.ITraceService
    {
#if DEBUG
        private void Log(string prefix, string message) => System.Diagnostics.Debug.WriteLine($"{prefix}: {message}");
        
#else
        private void Log(string prefix, string message) => System.Console.WriteLine($"{prefix}: {message}");
#endif
        public void Debug(string message, object payload = null) => Log("debug", message);
        public void Info(string message, object payload = null) => Log("INFO", message);
        public void Warn(string message, object payload = null) => Log("warn", message);
        public void Error(string message, object payload = null) => Log("error", message);
        public void Success(string message, object payload = null) => Log("success", message);
        public bool IsDebugEnabled { get; set; }
    }
}
