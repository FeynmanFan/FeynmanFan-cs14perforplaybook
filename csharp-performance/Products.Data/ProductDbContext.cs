using Microsoft.EntityFrameworkCore;

namespace Products.Data
{
    public class ProductDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();

        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}