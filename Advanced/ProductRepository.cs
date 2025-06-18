//// 1. Define interfaces (contracts)
//using Newtonsoft.Json;

//public interface IProductRepository
//{
//    Task<Product?> GetByIdAsync(int id);
//    Task<List<Product>> GetAllAsync();
//    Task<Product> CreateAsync(Product product);
//    Task UpdateAsync(Product product);
//    Task DeleteAsync(int id);
//}

//public interface IProductService
//{
//    Task<List<Product>> GetProductsWithErrorHandlingAsync(List<int> productIds);
//    Task<Product?> GetProductAsync(int id);
//}

//// 2. Repository implementation
//public class ProductRepository : IProductRepository
//{
//    private readonly HttpClient _httpClient;
//    private readonly ILogger<ProductRepository> _logger;

//    public ProductRepository(HttpClient httpClient, ILogger<ProductRepository> logger)
//    {
//        _httpClient = httpClient;
//        _logger = logger;
//    }

//    public async Task<Product?> GetByIdAsync(int id)
//    {
//        try
//        {
//            var response = await _httpClient.GetStringAsync($"api/products/{id}");
//            return JsonConvert.DeserializeObject<Product>(response);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Failed to get product {ProductId}", id);
//            return null;
//        }
//    }

//    // Other methods...
//}

//// 3. Service layer
//public class ProductService : IProductService
//{
//    private readonly IProductRepository _repository;
//    private readonly ILogger<ProductService> _logger;

//    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
//    {
//        _repository = repository;
//        _logger = logger;
//    }

//    public async Task<List<Product>> GetProductsWithErrorHandlingAsync(List<int> productIds)
//    {
//        var tasks = productIds.Select(id => _repository.GetByIdAsync(id));
//        var results = await Task.WhenAll(tasks);

//        return results.Where(p => p != null).ToList();
//    }
//}

