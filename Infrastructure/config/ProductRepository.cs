using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.config;

public class ProductRepository(StoreContext context) : IProductRepository
{
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }
    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort)
    {
        var query = context.Products.AsQueryable();

        if (!brand.IsNullOrEmpty())
            query = query.Where(w => w.Brand == brand);
        if (!type.IsNullOrEmpty())
            query = query.Where(w => w.Type == type);

        query = sort switch
        {
            "priceAsc" => query.OrderBy(o => o.Price),
            "priceDesc" => query.OrderByDescending(o => o.Price),
            _ => query.OrderBy(o => o.Name)
        };

        return await query.ToListAsync();
    }
    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await context.Products.Select(s => s.Brand)
                                        .Distinct().ToListAsync();
    }
    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await context.Products.Select(s => s.Type)
                                        .Distinct().ToListAsync();
    }
    public bool ProductExists(int id)
    {
        return context.Products.Any(x => x.Id == id);
    }
    public void AddProduct(Product product)
    {
        context.Products.Add(product);
    }
    public void UpdateProduct(Product product)
    {
        context.Entry(product).State = EntityState.Modified;
    }    
    public void DeleteProduct(Product product)
    {
        context.Products.Remove(product);
    }
    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
