using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BlazorGrpcWebApp.Server
{
    public class DataContextForMigrations : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DataContextForMigrations(DbContextOptions<DataContextForMigrations> options) : base(options) {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public DbSet<Unit> Units { get; set; }
        public DbSet<GrpcUnit> GrpcUnits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GrpcUser> GrpcUsers { get; set; }
    }
}
