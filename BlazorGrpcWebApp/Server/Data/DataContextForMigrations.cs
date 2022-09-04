using Microsoft.EntityFrameworkCore;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Enums;

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

            modelBuilder.Entity<LastActivity>()
                .Property(e => e.ActivityType)
                .HasConversion(
                    v => v.ToString(),
                    v => (Activity)Enum.Parse(typeof(Activity), v));
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserUnit> UserUnits { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<BattleLog> BattleLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<LastActivity> LastActivities { get; set; }
        public DbSet<UserLastActivitie> UserLastActivities { get; set; } 
    }
}
