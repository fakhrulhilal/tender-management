using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TenderManagement.Infrastructure.Identity;
using TenderManagement.Infrastructure.Persistence;

namespace TenderManagement.Infrastructure.Service
{
    public class Migration : IHostedService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Migration> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public Migration(ApplicationDbContext context, ILogger<Migration> logger,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_context.Database.IsSqlServer())
                    await _context.Database.MigrateAsync(cancellationToken);

                await ApplicationDbContextSeed.SeedDefaultUserAsync(_userManager, _roleManager);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while migrating or seeding the database with connection: {_context.Database.GetConnectionString()}.");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}