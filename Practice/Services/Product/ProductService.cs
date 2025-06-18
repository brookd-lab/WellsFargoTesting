using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        // Basic CRUD operations (delegating to repository)
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _repository.GetByCategory(category);
        }

        public async Task<List<Product>> GetExpensiveProductsAsync(decimal minPrice)
        {
            return await _repository.GetProductsAbovePrice(minPrice);
        }

        public async Task AddProductAsync(Product product)
        {
            // Business logic: Validation before adding
            ValidateProduct(product);
            await _repository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            ValidateProduct(product);
            await _repository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        // Business logic methods
        public async Task<bool> IsProductExpensiveAsync(int id, decimal threshold = 100m)
        {
            var product = await _repository.GetByIdAsync(id);
            return product.Price > threshold;
        }

        public async Task<decimal> GetAveragePriceAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Any() ? products.Average(p => p.Price) : 0m;
        }

        public async Task<List<Product>> GetDiscountedProductsAsync(decimal discountPercentage)
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => new Product(
                p.Id,
                p.Name,
                p.Price * (1 - discountPercentage / 100),
                p.Category
            )).ToList();
        }

        // Private validation method
        private void ValidateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be empty");

            if (product.Price < 0)
                throw new ArgumentException("Product price cannot be negative");

            if (string.IsNullOrWhiteSpace(product.Category))
                throw new ArgumentException("Product category cannot be empty");
        }
    }

}
