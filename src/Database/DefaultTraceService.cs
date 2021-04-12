namespace TenderManagement.Database
{
    public abstract class DefaultTraceService : Yuniql.Extensibility.ITraceService
    {
        protected abstract void Log(string prefix, string message);
        
        public void Debug(string message, object payload = null) => Log("debug", message);
        public void Info(string message, object payload = null) => Log("INFO", message);
        public void Warn(string message, object payload = null) => Log("warn", message);
        public void Error(string message, object payload = null) => Log("error", message);
        public void Success(string message, object payload = null) => Log("success", message);
        public bool IsDebugEnabled { get; set; }
    }
}
