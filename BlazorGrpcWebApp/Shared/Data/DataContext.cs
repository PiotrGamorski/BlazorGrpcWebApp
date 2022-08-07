using BlazorGrpcWebApp.Shared.Entities;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BlazorGrpcWebApp.Shared.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Battle>()
                .HasOne(b => b.Attacker)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Battle>()
                .HasOne(b => b.Opponent)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Battle>()
                .HasOne(b => b.Winner)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BattleLog>()
                .HasOne(b => b.Battle)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BattleLog>()
                .HasOne(b => b.Attacker)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BattleLog>()
                .HasOne(b => b.Opponent)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            //var converter = new ValueConverter<Timestamp, long >(
            //    v => v.ToDateTime().Ticks,
            //    v => Timestamp.FromDateTime(new DateTime(v, DateTimeKind.Utc)));

            //modelBuilder.Entity<GrpcUser>().Property(e => e.DateOfBirth).HasConversion(converter);
            //modelBuilder.Entity<GrpcUser>().Property(e => e.DateCreated).HasConversion(converter);
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserUnit> UserUnits { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<BattleLog> BattleLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
