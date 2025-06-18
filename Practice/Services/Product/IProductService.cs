using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Services.Product
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetProductsByCategoryAsync(string category);
        Task<List<Product>> GetExpensiveProductsAsync(decimal minPrice);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        // Business logic methods
        Task<bool> IsProductExpensiveAsync(int id, decimal threshold = 100m);
        Task<decimal> GetAveragePriceAsync();
        Task<List<Product>> GetDiscountedProductsAsync(decimal discountPercentage);
    }

}
