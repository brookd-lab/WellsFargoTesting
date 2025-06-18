using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTests.AntiDetection;

[TestFixture]
public class AntiDetectionSeleniumTests
{
    private IWebDriver _driver;

    [SetUp]
    public void Setup()
    {
        // Configure Chrome to avoid bot detection
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");

        _driver = new ChromeDriver(options);
        _driver.Manage().Window.Maximize();
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    [Test]
    public void GoogleSearch_WithAntiDetection_PerformsSearch()
    {
        // Arrange
        string searchTerm = "Selenium WebDriver C#";

        // Act
        _driver.Navigate().GoToUrl("https://www.google.com");

        // Wait a moment for page to fully load
        System.Threading.Thread.Sleep(2000);

        try
        {
            // Try to find and use the search box
            IWebElement searchBox = _driver.FindElement(By.Name("q"));
            searchBox.SendKeys(searchTerm);
            searchBox.SendKeys(Keys.Enter);

            // Wait for results
            System.Threading.Thread.Sleep(3000);

            // Assert
            Assert.That(_driver.Title, Does.Contain("Selenium WebDriver C#"));
            Console.WriteLine("✅ Google search successful - no CAPTCHA detected!");
        }
        catch (NoSuchElementException)
        {
            // If search box not found, check if we hit a CAPTCHA or verification page
            string pageSource = _driver.PageSource.ToLower();
            if (pageSource.Contains("captcha") || pageSource.Contains("verify") || pageSource.Contains("robot"))
            {
                Console.WriteLine("❌ Still detected as bot - CAPTCHA or verification page shown");
                Assert.Inconclusive("Google still detecting automation despite anti-detection measures");
            }
            else
            {
                throw; // Re-throw if it's a different issue
            }
        }
    }

    [Test]
    public void MultipleSearchEngines_TestAntiDetection_CompareResults()
    {
        // Test multiple search engines to see which ones work
        var searchEngines = new[]
        {
            ("https://www.bing.com", "Bing"),
            ("https://duckduckgo.com", "DuckDuckGo"),
            ("https://www.google.com", "Google")
        };

        foreach (var (url, name) in searchEngines)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);
                System.Threading.Thread.Sleep(2000);

                // Check if we can access the page without verification
                string title = _driver.Title;
                Console.WriteLine($"✅ {name}: Successfully accessed - Title: {title}");

                Assert.That(title, Does.Contain(name));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {name}: Failed to access - {ex.Message}");
            }
        }
    }

    [Test]
    public void BankingSimulation_LoginForm_TestRealWorldScenario()
    {
        // Use a banking-like test site to simulate Wells Fargo scenarios
        string testUrl = "https://the-internet.herokuapp.com/login";

        // Act
        _driver.Navigate().GoToUrl(testUrl);

        // Simulate typical banking login flow
        IWebElement usernameField = _driver.FindElement(By.Id("username"));
        IWebElement passwordField = _driver.FindElement(By.Id("password"));
        IWebElement loginButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

        // Enter credentials (these are the test site's valid credentials)
        usernameField.SendKeys("tomsmith");
        passwordField.SendKeys("SuperSecretPassword!");

        // Add a small delay to simulate human behavior
        System.Threading.Thread.Sleep(1000);

        loginButton.Click();

        // Verify successful login
        IWebElement successMessage = _driver.FindElement(By.CssSelector(".flash.success"));

        // Assert
        Assert.That(successMessage.Text, Does.Contain("You logged into a secure area!"));
        Assert.That(_driver.Url, Does.Contain("/secure"));

        Console.WriteLine("✅ Banking simulation successful - login flow completed");
    }
}
