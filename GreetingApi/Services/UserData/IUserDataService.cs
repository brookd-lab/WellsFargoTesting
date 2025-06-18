namespace GreetingApi.Services.UserData
{
    public interface IUserDataService
    {
        string GetUserRole(string username);
        DateTime GetLastLoginTime(string username);
    }

}
