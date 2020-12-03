using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

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

using AssignmentDesktopApp;

namespace Unit_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [UITestMethod]
        public void Valid_VehicleCreation()
        {
            // Arrange

            // Act

            MainApp mainApp = new MainApp();

            Vehicle vehicle = mainApp.CreateVehicle();

            // Assert

            Assert.IsNotNull(vehicle);
            
        }

        [UITestMethod]
        public void Valid_PumpCreation_FirstConstructor()
        {
            // Arrange

            int pumpNumber = 1;
            int expected = 1;

            // Act

            Pump pump = new Pump(pumpNumber);

            // Assert

            Assert.AreEqual(expected, pump.GetNum);

        }

        [UITestMethod]
        public void Valid_PumpCreation_SecondConstructor()
        {
            // Arrange

            int pumpNumber = 1;
            decimal dispenseRate = 4.5M;
            decimal expected = 4.5M;

            // Act

            Pump pump = new Pump(pumpNumber, dispenseRate);

            // Assert

            Assert.AreEqual(expected, pump.GetRate);

        }

        [UITestMethod]
        public void Fail_PumpCreation_SecondConstructor()
        {
            // Arrange

            int pumpNumber = -1;
            decimal dispenseRate = 4.5M;

            // Act

            // Assert

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                Pump pump = new Pump(pumpNumber, dispenseRate);
            });

        }
    }
}
