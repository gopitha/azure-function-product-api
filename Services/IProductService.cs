using ProductFunction.Models;

namespace ProductFunction.Services
{
    public interface IProductService
    {
        Task AddProductAsync(Product product);
        Task<List<Product>> GetProductsAsync();
    }
}
