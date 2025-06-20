namespace InventoryManagement
{
    public class LowStockReport
    {
        private const int SeverityLevelCritical = 5;
        private const int SeverityLevelWarning = 1;
        
        public Product Product { get; }
        public int UnitsBelow { get; }
        public SeverityLevel SeverityLevel { get; }

        public LowStockReport(Product product)
        {
            Product = product;
            UnitsBelow = Product.LowStockThreshold - Product.Quantity;
            if (UnitsBelow >= SeverityLevelCritical)
                SeverityLevel = SeverityLevel.Critical;
            else if (UnitsBelow >= SeverityLevelWarning)
                SeverityLevel = SeverityLevel.Warning;
            else
                SeverityLevel = SeverityLevel.Low; // For UnitsBelow = 0
        }
    }
}

