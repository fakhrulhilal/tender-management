using System.IO;
using System.Text.RegularExpressions;
using Yuniql.Core;

namespace TenderManagement.Database
{
    public static class Startup
    {
        public static void Main(string[] args)
        {
            var configuration = Configuration.Instance;
            configuration.Platform = "sqlserver";
            configuration.Workspace = GetDatabaseProjectDirectory(System.Environment.CurrentDirectory);
            configuration.ConnectionString = args[0];
            configuration.IsAutoCreateDatabase = true;

            var migrationServiceFactory = new MigrationServiceFactory(new DefaultTraceService());
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
    }
}