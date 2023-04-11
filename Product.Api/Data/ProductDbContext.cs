using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Product.Api.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<Entities.Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder, nameof(optionsBuilder));

            if (!optionsBuilder.IsConfigured)
            {
                _ = optionsBuilder.UseInMemoryDatabase(databaseName: "ProductDb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder, nameof(modelBuilder));

            _ = modelBuilder.Entity<Entities.Product>().HasKey(x => x.Id);

            _ = modelBuilder.Entity<Entities.Product>().HasData(
                new { Id = 1, Name = "Product One", Description = "", UnitPrice = 1.5m, CreatedOn = DateTime.UtcNow, IsAvailable = true },
                new { Id = 2, Name = "Product Two", Description = "", UnitPrice = 2.5m, CreatedOn = DateTime.UtcNow, IsAvailable = true },
                new { Id = 3, Name = "Product Three", Description = "", UnitPrice = 3.5m, CreatedOn = DateTime.UtcNow, IsAvailable = false }
            );

            _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
