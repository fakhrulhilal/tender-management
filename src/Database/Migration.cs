using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Yuniql.Core;
using Yuniql.Extensibility;

namespace TenderManagement.Database
{
    public class Migration : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly ITraceService _traceService;

        public Migration(IConfiguration configuration, ITraceService traceService)
        {
            _traceService = traceService ?? throw new ArgumentNullException(nameof(traceService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public static void Start(string connectionString, ITraceService traceService, string dbPath = null)
        {
            var configuration = Configuration.Instance;
            configuration.Platform = "sqlserver";
            if (!string.IsNullOrEmpty(dbPath))
                configuration.Workspace = dbPath;
            else
                configuration.Workspace = GetDatabaseProjectDirectory(Environment.CurrentDirectory);
            configuration.ConnectionString = connectionString;
            configuration.IsAutoCreateDatabase = true;

            var migrationServiceFactory = new MigrationServiceFactory(traceService);
            var migrationService = migrationServiceFactory.Create();
            migrationService.Run();
        }

        private static string GetDatabaseProjectDirectory(string currentDirectory)
        {
            char separator = Path.DirectorySeparatorChar;
            string regSeparator = Regex.Escape(separator.ToString());
            string projectDirPattern = $@"{regSeparator}bin({regSeparator}(x86|x64))?{regSeparator}(Debug|Release|\w+)({regSeparator}net[\w\.\-]+)?";
            string projectFolder = Regex.Replace(currentDirectory, projectDirPattern, string.Empty);
            projectFolder = Regex.Replace(projectFolder, $"({regSeparator}+)$", separator.ToString());
            string dbProjectDir = Path.Combine(projectFolder, $"..{separator}..{separator}src{separator}Database");
            return Path.GetFullPath(dbProjectDir);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            string dbConnectionString = _configuration.GetDatabaseConnection();
            Start(dbConnectionString, _traceService);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}