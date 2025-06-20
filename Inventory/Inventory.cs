using System.Collections.Generic;
using System.Linq;

namespace InventoryManagement
{
    public class Inventory : IInventory
    {
        private readonly List<Product> _products;

        public Inventory(List<Product> products = null)
        {
            if (products == null)
            {
                _products = new();
            }
            else
            {
                _products = new List<Product>(products!);
            }
        }

        public void AddProduct(Product product)
        {
            if (product is null)
                // More precise exception type:
                throw new ArgumentNullException(nameof(product));
            if (product.Id <= 0)
                throw new ArgumentException("Product must have a positive Id");

            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                throw new ArgumentException($"Product with ID {product.Id} already exists");
            }
            _products.Add(product);
        }

        public void UpdateProduct(Product oldProduct, Product newProduct)
        {
            var index = _products.IndexOf(oldProduct);
            if (index >= 0)
            {
                _products[index] = newProduct;
            }
        }

        public Product GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            foreach (var product in _products)
            {
                yield return product;
            }
        }

        public void DisplayAllProducts()
        {
            foreach (var product in _products)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Qty: {product.Quantity}, Price: ${product.Price}");
            }
        }

        public bool RemoveProduct(int productId)
        {
            var product = GetProductById(productId);
            if (product == null)
            {
                Console.WriteLine($"[WARNING] Product not found with id:{productId}");
                return false;
            }
            _products.Remove(product);
            Console.WriteLine($"[INFO] Removed product successfully with {productId}/{product.Name}");
            return true;
        }

        public Product UpdateProductQuantity(Product product, int amount)
        {
            if (amount < int.MinValue / 2 || amount > int.MaxValue / 2)
            {
                throw new ArgumentException("Amount is too large and could cause overflow");
            }

            var newQuantity = Math.Max(0, product.Quantity + amount);

            // Return a new Product with updated quantity
            return new Product(product.Id, product.Name, newQuantity, product.Price, product.LowStockThreshold);
        }

        public void Clear()
        {
            _products.Clear();            
        }
    }
}
