using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace SeleniumTests.BasicTests;

[TestFixture]
public class SeleniumBasicsTest
{
    private IWebDriver _driver;

    [SetUp]
    public void Setup()
    {
        // Initialize Chrome browser before each test
        _driver = new ChromeDriver();
        _driver.Manage().Window.Maximize();
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    [TearDown]
    public void TearDown()
    {
        // Close browser after each test
        _driver?.Quit();
        _driver?.Dispose();
    }

    [Test]
    public void OpenGoogle_SearchForSelenium_FindsResults()
    {
        // Arrange
        string searchTerm = "Selenium WebDriver";

        // Act
        _driver.Navigate().GoToUrl("https://www.google.com");

        // Find the search box and enter search term
        IWebElement searchBox = _driver.FindElement(By.Name("q"));
        searchBox.SendKeys(searchTerm);
        searchBox.SendKeys(Keys.Enter);

        // Wait for results and get the page title
        string pageTitle = _driver.Title;

        // Assert
        Assert.That(pageTitle, Does.Contain("Selenium WebDriver"));
        Assert.That(_driver.Url, Does.Contain("search"));
    }

    [Test]
    public void NavigateToWebsite_CheckTitle_VerifyCorrectPage()
    {
        // Act
        _driver.Navigate().GoToUrl("https://www.example.com");

        // Assert
        Assert.That(_driver.Title, Is.EqualTo("Example Domain"));
        Assert.That(_driver.Url, Is.EqualTo("https://www.example.com/"));
    }
}
