using Microsoft.EntityFrameworkCore;
using BlazorGrpcWebApp.Shared.Entities;

namespace BlazorGrpcWebApp.Server
{
    // USE THIS CONTEXT TO EXECUTE MIGRATIONS.
    // Place context in Program.cs and change context in related services.
    public class DataContextForMigrations : DbContext
    {
        public DataContextForMigrations(DbContextOptions<DataContextForMigrations> options) : base(options) {}

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
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserUnit> UserUnits { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<BattleLog> BattleLogs { get; set; }
    }
}
