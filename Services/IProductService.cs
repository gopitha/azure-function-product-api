using ProductFunction.Models;

namespace ProductFunction.Services
{
    public interface IProductService
    {
        Task AddProductAsync(Product product);
    }
}
