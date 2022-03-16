using BlazorGrpcWebApp.Shared.Entities;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BlazorGrpcWebApp.Shared.Data
{
    public class DataContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<Timestamp, long >(
                v => v.ToDateTime().Ticks,
                v => Timestamp.FromDateTime(new DateTime(v, DateTimeKind.Utc)));

            modelBuilder.Entity<GrpcUser>().Property(e => e.DateOfBirth).HasConversion(converter);
            modelBuilder.Entity<GrpcUser>().Property(e => e.DateCreated).HasConversion(converter);
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserUnit> UserUnits { get; set; }
        public DbSet<Battle> Battles { get; set; }
    }
}
