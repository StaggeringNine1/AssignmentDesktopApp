using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssignmentDesktopApp;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.Storage;
using Windows.ApplicationModel;

namespace Unit_Testing
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Valid_LoginID()
        {
            //Arrange

            int loginNumber = 1;
            bool expected = true;

            // Act

            MainApp mainApp = new MainApp();

            mainApp.Login(loginNumber, new TextBlock(), new Grid(), new Grid(), new Grid(), new Button());

            // Assert

            Assert.AreEqual(expected, mainApp.loggedIn, "Error logging in");
        }
    }
}
