var s1 = Singleton.Instance;
var s2 = Singleton.Instance;

public sealed class Singleton
{
    private static Singleton _instance;

    // Private constructor prevents instantiation from outside
    private Singleton()
    {
        // Initialize your singleton here
    }

    public static Singleton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Singleton();
            }
            return _instance;
        }
    }

    public void DoSomething()
    {
        Console.WriteLine("Singleton instance is doing something!");
    }
}