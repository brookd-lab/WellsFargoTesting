
namespace InventoryManagement
{
    public interface IInventory
    {
        void AddProduct(Product product);
        void Clear();
        void DisplayAllProducts();
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int id);
        bool RemoveProduct(int productId);
        void UpdateProduct(Product oldProduct, Product newProduct);
        Product UpdateProductQuantity(Product product, int amount);
    }
}