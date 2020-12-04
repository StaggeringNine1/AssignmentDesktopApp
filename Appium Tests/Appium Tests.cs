using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Appium;
using System;
using System.Threading;

namespace AppiumTest
{
    [TestClass]
    public class UnitTest1
    {
        private WindowsDriver<WindowsElement> driver;

        [TestInitialize]
        public void Initialisation()
        {
            AppiumOptions options = new AppiumOptions();

            options.AddAdditionalCapability("app", "FuelStationAssignment_cnj9vj70ehdf8!App");
            options.AddAdditionalCapability("deviceName", "WindowsPC");

            driver = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        private string GetCalculatorResultText()
        {
            return driver.FindElementByAccessibilityId("CalculatorResults").Text.Replace("Display is", string.Empty).Trim();
        }

        [TestMethod]
        [Obsolete]
        public void WrongAttendantID()
        {
            driver.FindElementByAccessibilityId("AttendantInput").Click();

            driver.Keyboard.SendKeys("1234");

            driver.FindElementByAccessibilityId("LoginButton").Click();

            driver.FindElementByAccessibilityId("Button0").Click();

            //string result = GetCalculatorResultText();

            //Assert.AreEqual("13", result);
        }
    }
}
