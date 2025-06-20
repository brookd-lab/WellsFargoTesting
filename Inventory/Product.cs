namespace InventoryManagement
{
    public class Product
    {
        public Product(int id, string name, int quantity, decimal price, int lowStockThreshold)
        {
            if (lowStockThreshold < 0)
                throw new ArgumentException("Low stock threshold must be >= 0");
            if (id <= 0) throw new ArgumentException("ID must be positive");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");
            if (price < 0) throw new ArgumentException("Price cannot be negative");

            Id = id;
            Name = name;
            Quantity = quantity;
            Price = price;
            LowStockThreshold = lowStockThreshold;
        }

        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; }
        public decimal Price { get; }
        public int LowStockThreshold { get; }
    }
}
