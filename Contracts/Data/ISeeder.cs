using portal_agile.Data;

namespace portal_agile.Contracts.Data
{
    public interface ISeeder
    {
        Task SeedAsync(AppDbContext context);
        int Order { get; }
    }
}
