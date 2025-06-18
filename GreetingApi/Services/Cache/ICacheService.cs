namespace GreetingApi.Services.Cache
{
    public interface ICacheService
    {
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        T? Get<T>(string key);
        bool Remove(string key);
        void Clear();
        int Count { get; }
    }

}
