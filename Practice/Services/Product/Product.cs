using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Services.Product;

public class Product
{
    public int Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public string Category { get; }

    public Product(int id, string name, decimal price, string category)
    {
        Id = id;
        Name = name;
        Price = price;
        Category = category;
    }
}
