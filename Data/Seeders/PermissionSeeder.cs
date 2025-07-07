using Microsoft.EntityFrameworkCore;
using portal_agile.Helpers;
using portal_agile.Security;

namespace portal_agile.Data.Seeders
{
    public class PermissionSeeder
    {
        public static readonly List<Permission> SeedData = new()
        {
           
        };

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasData(SeedData);
        }
    }
}
