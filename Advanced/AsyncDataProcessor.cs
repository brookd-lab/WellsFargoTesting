//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text.Json.Nodes;
//using System.Threading.Tasks;

//public class AsyncDataProcessor
//{
//    private static readonly HttpClient httpClient = new HttpClient();

//    // Sequential approach (slow)
//    public async Task<List<string>> ProcessUrlsSequentially(List<string> urls)
//    {
//        var results = new List<string>();

//        foreach (var url in urls)
//        {
//            var response = await httpClient.GetStringAsync(url);
//            results.Add(response.Substring(0, Math.Min(100, response.Length)));
//        }

//        return results;
//    }

//    // Concurrent approach (fast)
//    public async Task<List<string>> ProcessUrlsConcurrently(List<string> urls)
//    {
//        var tasks = urls.Select(async url =>
//        {
//            var response = await httpClient.GetStringAsync(url);
//            return response.Substring(0, Math.Min(100, response.Length));
//        });

//        var results = await Task.WhenAll(tasks);
//        return results.ToList();
//    }

//    private async Task<Product> GetSingleProductAsync(int productId, CancellationToken cancellationToken)
//    {
//        using var httpClient = new HttpClient();
//        var url = $"https://api.example.com/products/{productId}";

//        // Your task: Complete this method
//        var response = await httpClient.GetStringAsync(url, cancellationToken);
//        var product = JsonConvert.DeserializeObject<Product>(response);
//        return new Product { Id = productId, Name = $"Product {productId}" };
//    }

//    private async Task<Product?> GetSingleProductWithErrorHandling(int productId)
//    {
//        using var httpClient = new HttpClient();
//        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

//        try
//        {
//            var url = $"https://api.example.com/products/{productId}";
//            var response = await httpClient.GetStringAsync(url, cts.Token);
//            var product = JsonConvert.DeserializeObject<Product>(response);
//            return product;
//        }
//        catch (TaskCanceledException ex)
//        {
//            // Handle timeout - log and return null
//            Console.WriteLine($"Timeout for product {productId}: {ex.Message}");
//            // Could also log to your application's logging system
//            return null; // Don't throw - let other products continue
//        }
//        catch (HttpRequestException ex)
//        {
//            // Handle HTTP errors (404, 500, network issues)
//            Console.WriteLine($"HTTP error for product {productId}: {ex.Message}");
//            // Log the status code if available
//            return null; // Graceful degradation
//        }
//        catch (JsonException ex)
//        {
//            // Handle JSON parsing errors (malformed response)
//            Console.WriteLine($"JSON parsing error for product {productId}: {ex.Message}");
//            // This means API returned non-JSON or malformed JSON
//            return null; // Skip this product
//        }
//        catch (Exception ex)
//        {
//            // Catch any other unexpected exceptions
//            Console.WriteLine($"Unexpected error for product {productId}: {ex.Message}");
//            return null; // Fail gracefully
//        }
//    }


//    public async Task<List<Product?>> GetProductsWithErrorHandling(List<int> productIds)
//    {
//        // Create tasks for all products (they start immediately)
//        var tasks = productIds.Select(id => GetSingleProductWithErrorHandling(id));

//        // Wait for all tasks to complete concurrently
//        var results = await Task.WhenAll(tasks);

//        // Filter out null results (failed API calls) and return
//        return results.Where(p => p != null).ToList();
//    }
//}
