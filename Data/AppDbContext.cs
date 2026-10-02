using Microsoft.EntityFrameworkCore;
using WebApiLab4.Models;

namespace WebApiLab4.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Name).HasMaxLength(160).IsRequired();
            entity.Property(product => product.Category).HasMaxLength(100).IsRequired();
            entity.Property(product => product.Price).HasPrecision(12, 2);
            entity.HasData(
                new Product { Id = 1, Name = "Ноутбук", Price = 350000m, Category = "Электроника" },
                new Product { Id = 2, Name = "Наушники", Price = 42000m, Category = "Электроника" },
                new Product { Id = 3, Name = "Письменный стол", Price = 89000m, Category = "Мебель" });
        });
    }
}
