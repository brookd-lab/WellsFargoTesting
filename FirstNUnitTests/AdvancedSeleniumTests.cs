using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests.AdvancedFeatures;

[TestFixture]
public class AdvancedSeleniumTests
{
    private IWebDriver _driver;
    private WebDriverWait _wait;

    [SetUp]
    public void Setup()
    {
        // Your proven anti-detection configuration
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");

        _driver = new ChromeDriver(options);
        _driver.Manage().Window.Maximize();

        // Add explicit wait - much better than Thread.Sleep
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }


    [Test]
    public void ExplicitWaits_DynamicContent_HandlesLoadingStates()
    {
        // Navigate to a page with dynamic content
        _driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dynamic_loading/1");

        // Click start button
        IWebElement startButton = _driver.FindElement(By.CssSelector("#start button"));
        startButton.Click();

        // Improved explicit wait with longer timeout and visibility check
        WebDriverWait longWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        IWebElement finishText = longWait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(By.CssSelector("#finish h4"));
                return element.Displayed ? element : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });

        // Assert
        Assert.That(finishText.Text, Is.EqualTo("Hello World!"));
        Assert.That(finishText.Displayed, Is.True);

        Console.WriteLine("✅ Dynamic content loaded successfully with explicit wait");
    }



    [TestCase("tomsmith", "SuperSecretPassword!", true)]
    [TestCase("invaliduser", "wrongpassword", false)]
    [TestCase("", "", false)]
    public void ParameterizedLogin_VariousCredentials_HandlesAllScenarios(
        string username, string password, bool shouldSucceed)
    {
        // Navigate to login page
        _driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/login");

        // Fill login form
        _driver.FindElement(By.Id("username")).SendKeys(username);
        _driver.FindElement(By.Id("password")).SendKeys(password);
        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        if (shouldSucceed)
        {
            // Wait for success message
            IWebElement successMessage = _wait.Until(driver =>
                driver.FindElement(By.CssSelector(".flash.success")));

            Assert.That(successMessage.Text, Does.Contain("You logged into a secure area!"));
            Console.WriteLine($"✅ Login successful for user: {username}");
        }
        else
        {
            // Wait for error message
            IWebElement errorMessage = _wait.Until(driver =>
                driver.FindElement(By.CssSelector(".flash.error")));

            Assert.That(errorMessage.Text, Does.Contain("invalid"));
            Console.WriteLine($"✅ Login correctly failed for invalid credentials: {username}");
        }
    }

    [Test]
    public void DropdownHandling_SelectElements_DemonstratesAdvancedInteraction()
    {
        _driver.Navigate().GoToUrl("https://the-internet.herokuapp.com/dropdown");

        // Use SelectElement for proper dropdown handling
        IWebElement dropdownElement = _driver.FindElement(By.Id("dropdown"));
        SelectElement dropdown = new SelectElement(dropdownElement);

        // Test different selection methods
        dropdown.SelectByText("Option 1");
        Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Option 1"));

        dropdown.SelectByValue("2");
        Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Option 2"));

        Console.WriteLine("✅ Dropdown selection methods working correctly");
    }
}
