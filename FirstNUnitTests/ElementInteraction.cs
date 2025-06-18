using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTests.ElementInteraction;

[TestFixture]
public class WebElementInteractionTests
{
    private IWebDriver _driver;

    [SetUp]
    public void Setup()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);

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
    public void GoogleSearch_EnterSearchTerm_FindsResults()
    {
        // Arrange
        string searchTerm = "Selenium WebDriver";

        // Act
        _driver.Navigate().GoToUrl("https://www.google.com");

        // Find the search box and enter text
        IWebElement searchBox = _driver.FindElement(By.Name("q"));
        searchBox.SendKeys(searchTerm);
        searchBox.SendKeys(Keys.Enter);

        // Wait a moment for results to load
        System.Threading.Thread.Sleep(2000);

        // Assert
        Assert.That(_driver.Title, Does.Contain("Selenium WebDriver"));
        Assert.That(_driver.Url, Does.Contain("search"));
    }

    [Test]
    public void ExampleWebsite_CheckPageElements_VerifyContent()
    {
        // Act
        _driver.Navigate().GoToUrl("https://example.com");

        // Find the main heading
        IWebElement heading = _driver.FindElement(By.TagName("h1"));

        // Find the paragraph text
        IWebElement paragraph = _driver.FindElement(By.TagName("p"));

        // Assert
        Assert.That(heading.Text, Is.EqualTo("Example Domain"));
        Assert.That(paragraph.Text, Does.Contain("This domain is for use in illustrative examples"));
        Assert.That(_driver.Title, Is.EqualTo("Example Domain"));
    }
}
