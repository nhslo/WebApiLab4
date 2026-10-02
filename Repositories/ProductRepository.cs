using Microsoft.EntityFrameworkCore;
using WebApiLab4.Data;
using WebApiLab4.Models;

namespace WebApiLab4.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await context.Products.AsNoTracking().OrderBy(product => product.Id).ToListAsync();

    public Task<Product?> GetAsync(int id) =>
        context.Products.AsNoTracking().FirstOrDefaultAsync(product => product.Id == id);

    public async Task CreateAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product is null) return;
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }
}
