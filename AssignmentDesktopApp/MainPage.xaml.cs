using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
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
using Windows.UI.Composition;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AssignmentDesktopApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly MainApp mainApp = new MainApp();

        readonly List<ProgressBar> pumpBars = new List<ProgressBar>();
        readonly List<TextBlock> pumpStatus = new List<TextBlock>();
        readonly List<TextBlock> logoutText = new List<TextBlock>();
        readonly List<TextBlock> pumpInfoBox = new List<TextBlock>();
        readonly List<TextBlock> counters = new List<TextBlock>();

        public MainPage()
        {
            this.InitializeComponent();

            pumpBars.Add(Pump1Bar);
            pumpBars.Add(Pump2Bar);
            pumpBars.Add(Pump3Bar);
            pumpBars.Add(Pump4Bar);
            pumpBars.Add(Pump5Bar);
            pumpBars.Add(Pump6Bar);
            pumpBars.Add(Pump7Bar);
            pumpBars.Add(Pump8Bar);
            pumpBars.Add(Pump9Bar);

            pumpStatus.Add(Pump1Status);
            pumpStatus.Add(Pump2Status);
            pumpStatus.Add(Pump3Status);
            pumpStatus.Add(Pump4Status);
            pumpStatus.Add(Pump5Status);
            pumpStatus.Add(Pump6Status);
            pumpStatus.Add(Pump7Status);
            pumpStatus.Add(Pump8Status);
            pumpStatus.Add(Pump9Status);

            logoutText.Add(LogoutVehiclesServiced);
            logoutText.Add(LogoutVehiclesLeft);
            logoutText.Add(LogoutTotalLitres);
            logoutText.Add(LogoutTotalMoney);
            logoutText.Add(LogoutCommission);

            pumpInfoBox.Add(PumpNumber);
            pumpInfoBox.Add(PumpVehicles);
            pumpInfoBox.Add(PumpRate);
            pumpInfoBox.Add(PumpLitres);

            counters.Add(VehiclesQueuing);
            counters.Add(VehiclesServiced);
            counters.Add(VehiclesLeft);
            counters.Add(TotalLitres);
            counters.Add(TotalMoney);

            mainApp.Initialise(pumpBars, pumpStatus, counters);
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            TextBox inputText = AttendantInput;

            if (inputText.Text != "")
            {
                mainApp.Login(Convert.ToInt32(inputText.Text), UserWelcome, LoginGrid, PumpGrid, CounterGrid, LogoutButton, AttendantInput);
            }
        }

        private async void AttendantInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex pattern = new Regex(@"^[0-9]*$");

            string text = (sender as TextBox).Text;

            if (!pattern.IsMatch(text))
            {
                MessageDialog messageDialog = new MessageDialog("Wrong")
                {
                    Title = "Wrong input detected!",
                    Content = "Only numbers (0-9) are in your attendant ID"
                };
                await messageDialog.ShowAsync();

                (sender as TextBox).Text = "";
            }
        }

        private void PumpSelect(object sender, RoutedEventArgs e)
        {
            string accessKey = (sender as Button).AccessKey;

            mainApp.SelectPump(accessKey);
        }

        private void PumpInfo(object sender, RoutedEventArgs e)
        {
            string accessKey = (sender as Button).AccessKey;

            mainApp.ViewPumpInfo(accessKey, PumpInformation, pumpInfoBox);
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            mainApp.Logout(PumpGrid, CounterGrid, LogoutGrid, LogoutButton, logoutText);
        }

        private void InformationClose(object sender, RoutedEventArgs e)
        {
            PumpInformation.Visibility = Visibility.Collapsed;
        }

        private void LogoutContinue(object sender, RoutedEventArgs e)
        {
            LogoutGrid.Visibility = Visibility.Collapsed;
            UserWelcome.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;

            AttendantInput.Text = "";
        }
    }
}
