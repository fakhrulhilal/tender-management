using System;
using Microsoft.Extensions.Logging;

namespace TenderManagement.Database
{
    public class LogTraceService : Yuniql.Extensibility.ITraceService
    {
        private readonly ILogger<Migration> _logger;

        public LogTraceService(ILogger<Migration> logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public void Debug(string message, object payload = null) => _logger.LogDebug(message, payload);
        public void Info(string message, object payload = null) => _logger.LogInformation(message, payload);
        public void Warn(string message, object payload = null) => _logger.LogWarning(message, payload);
        public void Error(string message, object payload = null) => _logger.LogError(message, payload);
        public void Success(string message, object payload = null) => _logger.LogInformation(message, payload);
        public bool IsDebugEnabled { get; set; }
    }
}
