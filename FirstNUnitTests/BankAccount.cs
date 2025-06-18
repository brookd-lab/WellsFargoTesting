namespace BankAccount;

[TestFixture]
public class CombinedExample
{
    private BankAccount _account;

    [SetUp]
    public void Setup()
    {
        // Create a fresh account with $1000 for each test
        _account = new BankAccount(1000.00m);
    }

    [TearDown]
    public void TearDown()
    {
        _account = null;
    }

    [TestCase(100.00, 1100.00)]
    [TestCase(250.50, 1250.50)]
    [TestCase(0.01, 1000.01)]
    [TestCase(999.99, 1999.99)]
    public void Deposit_VariousAmounts_IncreasesBalance(decimal depositAmount, decimal expectedBalance)
    {
        // Act
        _account.Deposit(depositAmount);

        // Assert
        Assert.That(_account.Balance, Is.EqualTo(expectedBalance));
    }

    [TestCase(100.00, 900.00)]
    [TestCase(500.00, 500.00)]
    [TestCase(1000.00, 0.00)]
    public void Withdraw_ValidAmounts_DecreasesBalance(decimal withdrawAmount, decimal expectedBalance)
    {
        // Act
        _account.Withdraw(withdrawAmount);

        // Assert
        Assert.That(_account.Balance, Is.EqualTo(expectedBalance));
    }
}

// Simple BankAccount class for testing
public class BankAccount
{
    public decimal Balance { get; private set; }

    public BankAccount(decimal initialBalance)
    {
        Balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive");
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive");
        if (amount > Balance)
            throw new InvalidOperationException("Insufficient funds");
        Balance -= amount;
    }
}
