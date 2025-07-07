using Microsoft.EntityFrameworkCore;

namespace portal_agile.Helpers
{
    public static class SeederHelper
    {
        private static readonly Dictionary<Type, List<object>> _seededEntities = new();

        public static T[] Seed<T>(this ModelBuilder modelBuilder, params T[] entities) where T : class
        {
            // Store the seeded entities for later reference
            if (!_seededEntities.ContainsKey(typeof(T)))
                _seededEntities[typeof(T)] = new List<object>();

            _seededEntities[typeof(T)].AddRange(entities.Cast<object>());

            // Seed to EF Core
            modelBuilder.Entity<T>().HasData(entities);

            return entities;
        }

        public static T[] GetSeeded<T>() where T : class
        {
            return _seededEntities.ContainsKey(typeof(T))
                ? _seededEntities[typeof(T)].Cast<T>().ToArray()
                : Array.Empty<T>();
        }

        public static T GetSeeded<T>(Func<T, bool> predicate) where T : class
        {
            return GetSeeded<T>().FirstOrDefault(predicate)!;
        }
    }
}
