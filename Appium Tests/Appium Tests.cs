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
        [Obsolete]
        public void Initialisation()
        {
            AppiumOptions options = new AppiumOptions();

            options.AddAdditionalCapability("app", "FuelStationAssignment_cnj9vj70ehdf8!App");
            options.AddAdditionalCapability("deviceName", "WindowsPC");

            driver = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Obsolete]
        private void SuccessLogin()
        {
            driver.FindElementByAccessibilityId("AttendantInput").Click();

            driver.Keyboard.SendKeys("1");

            driver.FindElementByAccessibilityId("LoginButton").Click();
        }

        [TestMethod]
        [Obsolete]
        public void Invalid_AttendantID()
        {
            driver.FindElementByAccessibilityId("AttendantInput").Click();

            driver.Keyboard.SendKeys("1234");

            driver.FindElementByAccessibilityId("LoginButton").Click();

            driver.FindElementByAccessibilityId("Button0").Click();
        }

        [TestMethod]
        [Obsolete]
        public void Letters_AttendantID()
        {
            driver.FindElementByAccessibilityId("AttendantInput").Click();

            driver.Keyboard.SendKeys("g");

            driver.FindElementByAccessibilityId("Button0").Click();
        }

        [TestMethod]
        [Obsolete]
        public void Login_Logout()
        {
            SuccessLogin();

            driver.FindElementByAccessibilityId("LogoutButton").Click();

            driver.FindElementByAccessibilityId("Continue").Click();
        }

        [TestMethod]
        [Obsolete]
        public void Pump6Select()
        {
            SuccessLogin();

            driver.FindElementByAccessibilityId("SelectPump6").Click();

            driver.CloseApp();
        }

        [TestMethod]
        [Obsolete]
        public void SelectPump_Wait_ThenLogout()
        {
            SuccessLogin();

            driver.FindElementByAccessibilityId("SelectPump6").Click();

            Thread.Sleep(25000);

            driver.FindElementByAccessibilityId("LogoutButton").Click();

            driver.FindElementByAccessibilityId("Continue").Click();
        }
    }
}
