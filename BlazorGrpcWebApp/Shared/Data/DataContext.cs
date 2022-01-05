using BlazorGrpcWebApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Shared.Data
{
    public class DataContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public DbSet<Unit> Units { get; set; }
        public DbSet<GrpcUnit> GrpcUnits { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
