using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TenderManagement.Database
{
    public static class DatabaseConnectionBuilder
    {
        public static readonly Options Keys = new ("server", "name", "username", "password");
        
        public static string GetDatabaseConnection(this IConfiguration config)
        {
            string template = config.GetValue<string>("ConnectionStrings:DefaultConnection");
            var builder = new SqlConnectionStringBuilder(template);
            string server = config.GetValue<string>($"ConnectionStrings:{Keys.Server}");
            if (!string.IsNullOrEmpty(server))
                builder.DataSource = server;
            string dbName = config.GetValue<string>($"ConnectionStrings:{Keys.Name}");
            if (!string.IsNullOrEmpty(dbName))
                builder.InitialCatalog = dbName;
            string username = config.GetValue<string>($"ConnectionStrings:{Keys.Username}");
            if (!string.IsNullOrEmpty(username))
                builder.UserID = username;
            string password = config.GetValue<string>($"ConnectionStrings:{Keys.Password}");
            if (!string.IsNullOrEmpty(password))
                builder.Password = password;
            return builder.ConnectionString;
        }

        public record Options(string Server, string Name, string Username, string Password);
    }
}
