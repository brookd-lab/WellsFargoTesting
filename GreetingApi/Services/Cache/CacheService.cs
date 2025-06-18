using System.Collections.Concurrent;

namespace GreetingApi.Services.Cache;

public class CacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache;
    private readonly int _instanceId;
    private static int _instanceCounter = 0;

    public CacheService()
    {
        _instanceId = ++_instanceCounter;
        _cache = new ConcurrentDictionary<string, CacheItem>();
        Console.WriteLine($"CacheService instance #{_instanceId} created at {DateTime.Now:HH:mm:ss.fff}");
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var expirationTime = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : (DateTime?)null;
        var item = new CacheItem(value, expirationTime);

        _cache.AddOrUpdate(key, item, (k, v) => item);
        Console.WriteLine($"Cache SET: {key} by thread {Thread.CurrentThread.ManagedThreadId}");
    }

    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            // Check expiration
            if (item.ExpirationTime.HasValue && DateTime.UtcNow > item.ExpirationTime.Value)
            {
                _cache.TryRemove(key, out _);
                Console.WriteLine($"Cache EXPIRED: {key} by thread {Thread.CurrentThread.ManagedThreadId}");
                return default(T);
            }

            Console.WriteLine($"Cache HIT: {key} by thread {Thread.CurrentThread.ManagedThreadId}");
            return (T)item.Value;
        }

        Console.WriteLine($"Cache MISS: {key} by thread {Thread.CurrentThread.ManagedThreadId}");
        return default(T);
    }

    public bool Remove(string key)
    {
        var removed = _cache.TryRemove(key, out _);
        if (removed)
            Console.WriteLine($"Cache REMOVE: {key} by thread {Thread.CurrentThread.ManagedThreadId}");
        return removed;
    }

    public void Clear()
    {
        _cache.Clear();
        Console.WriteLine($"Cache CLEARED by thread {Thread.CurrentThread.ManagedThreadId}");
    }

    public int Count => _cache.Count;

    private record CacheItem(object Value, DateTime? ExpirationTime);
}
