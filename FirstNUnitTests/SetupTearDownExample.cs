using NUnit.Framework;

namespace SetUpTearDown;

[TestFixture]
public class SetupTearDownExample
{
    private Calculator _calculator;
    private List<string> _testLog;

    [SetUp]  // Runs BEFORE each individual test
    public void Setup()
    {
        Console.WriteLine("Setting up test...");
        _calculator = new Calculator();
        _testLog = new List<string>();
        _testLog.Add("Test started");
    }

    [TearDown]  // Runs AFTER each individual test
    public void TearDown()
    {
        Console.WriteLine("Cleaning up test...");
        _testLog.Add("Test completed");
        _calculator = null;
        _testLog = null;
    }

    [Test]
    public void Add_TwoNumbers_ReturnsSum()
    {
        // Arrange (calculator is already set up)
        _testLog.Add("Testing addition");

        // Act
        int result = _calculator.Add(5, 3);

        // Assert
        Assert.That(result, Is.EqualTo(8));
        Assert.That(_testLog, Has.Count.EqualTo(2)); // "Test started" + "Testing addition"
    }

    [Test]
    public void Multiply_TwoNumbers_ReturnsProduct()
    {
        // Arrange (calculator is already set up)
        _testLog.Add("Testing multiplication");

        // Act
        int result = _calculator.Multiply(4, 6);

        // Assert
        Assert.That(result, Is.EqualTo(24));
        Assert.That(_testLog, Has.Count.EqualTo(2)); // Fresh setup for each test
    }
}

// Simple Calculator class for testing
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Multiply(int a, int b) => a * b;
    public double Divide(double a, double b) => a / b;
}
