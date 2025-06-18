using GreetingApi.Services.Cache;
using GreetingApi.Services.UserData;

public class UserDataService : IUserDataService
{
    private readonly ICacheService _cacheService;
    private readonly int _instanceId;
    private static int _instanceCounter = 0;

    public UserDataService(ICacheService cacheService)
    {
        _instanceId = ++_instanceCounter;
        _cacheService = cacheService;
        Console.WriteLine($"UserDataService instance #{_instanceId} created with cache dependency at {DateTime.Now:HH:mm:ss.fff}");
    }

    public string GetUserRole(string username)
    {
        var cacheKey = $"role_{username}";

        // Try cache first
        var cachedRole = _cacheService.Get<string>(cacheKey);
        if (cachedRole != null)
            return cachedRole;

        // Simulate expensive database lookup
        Thread.Sleep(100); // Simulate delay
        var role = username.ToLower() switch
        {
            "admin" => "Administrator",
            "john" => "Manager",
            "jane" => "Employee",
            _ => "Guest"
        };

        // Cache for 30 seconds
        _cacheService.Set(cacheKey, role, TimeSpan.FromSeconds(30));
        return role;
    }

    public DateTime GetLastLoginTime(string username)
    {
        var cacheKey = $"login_{username}";

        var cachedTime = _cacheService.Get<DateTime?>(cacheKey);
        if (cachedTime.HasValue)
            return cachedTime.Value;

        // Simulate database lookup
        var loginTime = DateTime.Now.AddHours(-new Random().Next(1, 48));
        _cacheService.Set(cacheKey, loginTime, TimeSpan.FromSeconds(30));
        return loginTime;
    }
}
