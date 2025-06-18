using GreetingApi.Services.UserData;

namespace GreetingApi.Services.Greeting;

public class GreetingService : IGreetingService
{
    private readonly int _instanceId;
    private static int _instanceCounter = 0;
    private readonly IUserDataService _userDataService;

    // Constructor injection - IUserDataService will be injected automatically
    public GreetingService(IUserDataService userDataService)
    {
        _instanceId = ++_instanceCounter;
        _userDataService = userDataService;
        Console.WriteLine($"GreetingService instance #{_instanceId} created with UserDataService dependency at {DateTime.Now:HH:mm:ss.fff}");
    }

    public string GetGreeting(string name)
    {
        var role = _userDataService.GetUserRole(name);
        var lastLogin = _userDataService.GetLastLoginTime(name);

        return $"Hello, {name}! You are a {role}. Last login: {lastLogin:yyyy-MM-dd HH:mm}. (GreetingService instance #{_instanceId})";
    }
}
