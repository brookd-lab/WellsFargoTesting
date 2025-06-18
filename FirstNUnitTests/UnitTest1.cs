using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ParameterizedTests;

[TestFixture]
public class MyFirstTest
{
    [Test]
    public void SimpleAddition_ShouldWork()
    {
        // Arrange
        int a = 2;
        int b = 3;

        // Act
        int result = a + b;

        // Assert
        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void StringComparison_ShouldWork()
    {
        // Arrange
        string firstName = "John";
        string lastName = "Doe";

        // Act
        string fullName = $"{firstName} {lastName}";

        // Assert
        Assert.That(fullName, Is.EqualTo("John Doe"));
        Assert.That(fullName, Does.Contain("John"));
        Assert.That(fullName, Does.StartWith("John"));
    }

    [Test]
    public void NumberComparisons_ShouldWork()
    {
        // Arrange
        int score = 85;

        // Assert
        Assert.That(score, Is.GreaterThan(80));
        Assert.That(score, Is.LessThan(90));
        Assert.That(score, Is.InRange(80, 90));
    }

    [Test]
    public void CollectionTests_ShouldWork()
    {
        // Arrange
        var numbers = new List<int> { 1, 2, 3, 4, 5 };

        // Assert
        Assert.That(numbers, Has.Count.EqualTo(5));
        Assert.That(numbers, Does.Contain(3));
        Assert.That(numbers, Is.All.GreaterThan(0));
    }

    /// Test the same addition logic with multiple inputs
    [TestCase(2, 3, 5)]
    [TestCase(10, 15, 25)]
    [TestCase(-1, 1, 0)]
    [TestCase(0, 0, 0)]
    [TestCase(100, 200, 300)]
    public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
    {
        // Act
        int result = a + b;

        // Assert
        Assert.That(result, Is.EqualTo(expected),
            $"Adding {a} + {b} should equal {expected}");
    }

    // Test string operations with different inputs
    [TestCase("hello", "HELLO")]
    [TestCase("World", "WORLD")]
    [TestCase("NUnit", "NUNIT")]
    [TestCase("", "")]
    public void ToUpper_VariousStrings_ReturnsUpperCase(string input, string expected)
    {
        // Act
        string result = input.ToUpper();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    // Test boolean conditions
    [TestCase(2, true)]
    [TestCase(3, true)]
    [TestCase(5, true)]
    [TestCase(7, true)]
    [TestCase(4, false)]
    [TestCase(6, false)]
    [TestCase(8, false)]
    [TestCase(1, false)]
    public void IsPrime_VariousNumbers_ReturnsCorrectResult(int number, bool expected)
    {
        // Act
        bool result = IsPrime(number);

        // Assert
        Assert.That(result, Is.EqualTo(expected),
            $"{number} should {(expected ? "" : "not ")}be prime");
    }

    // Helper method for prime number testing
    private bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    [TestCase(100, 10, ExpectedResult = 10, TestName = "Divide 100 by 10")]
    [TestCase(50, 5, ExpectedResult = 10, TestName = "Divide 50 by 5")]
    [TestCase(20, 4, ExpectedResult = 5, TestName = "Divide 20 by 4")]
    public double Divide_VariousInputs_ReturnsQuotient(double dividend, double divisor)
    {
        // Act & Return (ExpectedResult handles the assertion)
        return dividend / divisor;
    }
}
