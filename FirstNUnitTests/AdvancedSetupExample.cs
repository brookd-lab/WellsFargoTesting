using SetUpTearDown;

namespace Database;

[TestFixture]
public class AdvancedSetupExample
{
    private static DatabaseConnection _database; // Expensive to create
    private Calculator _calculator; // Cheap to create

    [OneTimeSetUp]  // Runs ONCE before ALL tests in this class
    public void OneTimeSetup()
    {
        Console.WriteLine("One-time setup: Creating database connection...");
        _database = new DatabaseConnection("test_connection_string");
    }

    [OneTimeTearDown]  // Runs ONCE after ALL tests in this class
    public void OneTimeTearDown()
    {
        Console.WriteLine("One-time teardown: Closing database connection...");
        _database?.Close();
        _database = null;
    }

    [SetUp]  // Runs before EACH test
    public void Setup()
    {
        Console.WriteLine("Per-test setup: Creating calculator...");
        _calculator = new Calculator();
    }

    [TearDown]  // Runs after EACH test
    public void TearDown()
    {
        Console.WriteLine("Per-test teardown: Disposing calculator...");
        _calculator = null;
    }

    [Test]
    public void DatabaseCalculation_Test1()
    {
        // Both _database and _calculator are available
        int result = _calculator.Add(1, 2);
        Assert.That(result, Is.EqualTo(3));
        Assert.That(_database, Is.Not.Null);
    }

    [Test]
    public void DatabaseCalculation_Test2()
    {
        // Fresh _calculator, same _database
        int result = _calculator.Multiply(2, 3);
        Assert.That(result, Is.EqualTo(6));
        Assert.That(_database, Is.Not.Null);
    }
}

// Mock database class for demonstration
public class DatabaseConnection
{
    public string ConnectionString { get; }

    public DatabaseConnection(string connectionString)
    {
        ConnectionString = connectionString;
        Console.WriteLine($"Database connected: {connectionString}");
    }

    public void Close()
    {
        Console.WriteLine("Database connection closed");
    }
}
