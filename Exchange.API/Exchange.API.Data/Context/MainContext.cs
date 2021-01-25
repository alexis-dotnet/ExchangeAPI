using Exchange.API.Data.Entities;
using Exchange.API.Data.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Exchange.API.Data.Context
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PurchaseMap());
            modelBuilder.ApplyConfiguration(new UserMap());
        }

        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
