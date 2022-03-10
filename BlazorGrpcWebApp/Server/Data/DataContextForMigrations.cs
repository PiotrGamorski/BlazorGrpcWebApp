using Microsoft.EntityFrameworkCore;
using BlazorGrpcWebApp.Shared.Entities;

namespace BlazorGrpcWebApp.Server
{
    // USE THIS CONTEXT TO EXECUTE MIGRATIONS.
    // Place context in Program.cs and change related services.
    public class DataContextForMigrations : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DataContextForMigrations(DbContextOptions<DataContextForMigrations> options) : base(options) {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserUnit> UserUnits { get; set; }
    }
}
