using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTests.BasicTests;

[TestFixture]
public class MyFirstSeleniumTest
{
    private IWebDriver _driver;

    [SetUp]
    public void Setup()
    {
        // Create a new Chrome browser instance before each test
        _driver = new ChromeDriver();
    }

    [TearDown]
    public void TearDown()
    {
        // Close the browser after each test
        _driver?.Quit();
        _driver?.Dispose();
    }

    [Test]
    public void OpenBrowser_NavigateToGoogle_VerifyTitle()
    {
        // Act - Navigate to Google
        _driver.Navigate().GoToUrl("https://www.google.com");

        // Assert - Check that we're on Google
        Assert.That(_driver.Title, Does.Contain("Google"));
    }
}
