using Microsoft.AspNetCore.Identity;
using portal_agile.Contracts.Data;
using portal_agile.Data;
using portal_agile.Data.Seeders;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Services
{
    public class SeederManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeederManager> _logger;

        public SeederManager(IServiceProvider serviceProvider, ILogger<SeederManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                // Get all seeder implementations
                var seeders = _serviceProvider.GetServices<ISeeder>()
                                              .OrderBy(s => s.Order)
                                              .ToList();

                _logger.LogInformation("Starting database seeding...");

                foreach (var seeder in seeders)
                {
                    _logger.LogInformation($"Running seeder: {seeder.GetType().Name}");
                    await seeder.SeedAsync(context);
                    _logger.LogInformation($"Completed seeder: {seeder.GetType().Name}");
                }

                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
