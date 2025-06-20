using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement
{
    public class LowStockReportService
    {
        private readonly IInventory _inventory;

        public LowStockReportService(IInventory inventory)
        {
            _inventory = inventory;
        }

        public IEnumerable<Product> CheckLowStock()
        {
            return _inventory.GetAllProducts().Where(p => p.Quantity <= p.LowStockThreshold);
        }

        public IEnumerable<LowStockReport> GetLowStockReport()
        {
            var productsWithLowStock = CheckLowStock();
            foreach (var product in productsWithLowStock)
            {
                yield return new LowStockReport(product);
            }
        }

        public void DisplayLowStockReport()
        {
            var lowStockReport = GetLowStockReport();

            Console.WriteLine("Displaying low stock report");

            if (!lowStockReport.Any())
            {
                Console.WriteLine("No stock report to display");
                return;
            }

            foreach (var report in lowStockReport)
            {
                Console.WriteLine($"{report.Product.Name}, Current quantity/threshold: {report.Product.Quantity}/{report.Product.LowStockThreshold}, units below threshold: {report.UnitsBelow}, severity: {report.SeverityLevel}");
            }
        }

    }
}
