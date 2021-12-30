using BlazorGrpcWebApp.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Unit> Units { get; set; }
    }
}
