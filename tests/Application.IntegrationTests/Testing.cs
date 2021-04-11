using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Respawn;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TenderManagement.Application.Common.Port;
using TenderManagement.Domain.Common;
using TenderManagement.Infrastructure.Identity;
using TenderManagement.Infrastructure.Persistence;
using TenderManagement.WebApi;

namespace TenderManagement.Application.IntegrationTests
{
    [SetUpFixture]
    public class Testing
    {
        private static IConfigurationRoot _configuration;
        private static IServiceScopeFactory _scopeFactory;
        private static Checkpoint _checkpoint;
        private static string _currentUserId;
        internal static ServiceCollection ServiceCollection { get; private set; }
        internal static DateTime CurrentDateTime { get; set; }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            var startup = new Startup(_configuration);

            ServiceCollection = new ServiceCollection();

            ServiceCollection.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
                w.EnvironmentName == "Development" &&
                w.ApplicationName == "TenderManagement.WebApi"));

            ServiceCollection.AddLogging();

            startup.ConfigureServices(ServiceCollection);

            // Replace service registration for ICurrentUserService
            // Remove existing registration
            var currentUserServiceDescriptor = ServiceCollection.FirstOrDefault(d =>
                d.ServiceType == typeof(ICurrentUserService));
            ServiceCollection.Remove(currentUserServiceDescriptor);
            ServiceCollection.AddTransient(_ => Mock.Of<ICurrentUserService>(s => s.UserId == _currentUserId));

            // ensure we use English for validation message
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            // change implementation of clock with mocked version
            ServiceCollection.RemoveAll(typeof(IDateTime));
            var dateTimeMock = new Mock<IDateTime>();
            CurrentDateTime = DateTime.Now;
            dateTimeMock.Setup(p => p.Now).Returns(CurrentDateTime);
            ServiceCollection.AddSingleton(dateTimeMock.Object);

            _scopeFactory = ServiceCollection.BuildServiceProvider().GetService<IServiceScopeFactory>();

            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };

            EnsureDatabase();
        }

        private static void EnsureDatabase()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            Database.Startup.Main(new[] { context.Database.GetConnectionString() });
        }

        public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
            return await mediator.Send(request);
        }

        public static void ConfigureNow(DateTime now) => CurrentDateTime = now;

        public static async Task<string> RunAsDefaultUserAsync()
        {
            return await RunAsUserAsync("test@local", "Testing1234!", new string[] { });
        }

        public static async Task<string> RunAsAdministratorAsync()
        {
            return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { "Administrator" });
        }

        public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
        {
            using var scope = _scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser { UserName = userName, Email = userName };
            var result = await userManager.CreateAsync(user, password);
            if (roles.Any())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                await userManager.AddToRolesAsync(user, roles);
            }

            if (result.Succeeded)
            {
                _currentUserId = user.Id;
                return _currentUserId;
            }

            var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);
            throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
        }

        public static async Task ResetState()
        {
            await _checkpoint.Reset(_configuration.GetConnectionString("DefaultConnection"));
            _currentUserId = null;
        }

        public static async Task<TEntity> FindAsync<TEntity>(params object[] keyValues)
            where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.FindAsync<TEntity>(keyValues);
        }

        public static async Task AddAsync<TEntity>(bool enableIdentityInsert, params TEntity[] entities)
            where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            foreach (var entity in entities)
            {
                if (entity is AuditableEntity auditableEntity)
                {
                    if (auditableEntity.Created == default) auditableEntity.Created = DateTime.Now;
                    if (string.IsNullOrEmpty(auditableEntity.CreatedBy)) auditableEntity.CreatedBy = typeof(Testing).FullName;
                }

                context.Add(entity);
            }
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            string tableName = $"{entityType.GetSchema() ?? "dbo"}.{entityType.GetTableName()}";
            await using var transaction = await context.Database.BeginTransactionAsync();
            if (enableIdentityInsert) context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} ON;");
            await context.SaveChangesAsync();
            if (enableIdentityInsert) context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} OFF;");
            await transaction.CommitAsync();
        }

        public static async Task<int> CountAsync<TEntity>() where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.Set<TEntity>().CountAsync();
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
        }
    }
}
