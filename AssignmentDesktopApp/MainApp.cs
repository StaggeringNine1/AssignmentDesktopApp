using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

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

namespace AssignmentDesktopApp
{
    public class MainApp
    {
        readonly CoreWindow window = CoreWindow.GetForCurrentThread();
        readonly CancellationTokenSource cancelToken = new CancellationTokenSource();
        readonly Random random = new Random();

        decimal totalLitresDispensed;
        decimal totalMoneyEarned;
        decimal commission;

        int attendantId;
        int numberVehiclesServiced;
        int numberVehiclesLeft;

        public bool loggedIn;

        readonly List<Vehicle> allVehicles = new List<Vehicle>();
        readonly List<Vehicle> queuingVehicles = new List<Vehicle>();

        List<ProgressBar> pumpBars = new List<ProgressBar>();
        List<TextBlock> pumpStatuses = new List<TextBlock>();
        List<TextBlock> counterBlocks = new List<TextBlock>();

        readonly Dictionary<int, Pump> allPumps = new Dictionary<int, Pump>();

        readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

        /// <summary>
        /// Change the status of all current pumps in the program
        /// </summary>
        /// <param name="state">"add" for adding a vehicle, "leave" for removing a vehicle from the forecourt</param>
        /// <param name="pumpNumber">The pumpnumber that is being occupied/vehicle removed from</param>
        /// <param name="dispatcher">The CoreDispatcher to update the UI for the correct thread</param>
        private void ChangePumps(string state, int pumpNumber, CoreDispatcher dispatcher)
        {
            Task pumpControl = new Task(() => { ControlPumpText(dispatcher); });

            switch (state)
            {
                case "add":
                    foreach (int key in allPumps.Keys.ToList())
                    {
                        if (key == pumpNumber)
                        {
                            if (!allPumps[key].GetInUse)
                            {
                                allPumps[key].IsInUse = true;

                                if (key != 3 && key != 6 && key != 9)
                                {
                                    if (!allPumps[key + 1].GetInUse)
                                    {
                                        allPumps[key + 1].IsInUse = true;

                                        if (key != 2 && key != 5 && key != 8)
                                        {
                                            if (!allPumps[key + 2].GetInUse)
                                            {
                                                allPumps[key + 2].IsInUse = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    pumpControl.Start();
                    pumpControl.Wait();

                    break;

                case "leave":
                    foreach (int key in allPumps.Keys.ToList())
                    {
                        if (key == pumpNumber)
                        {
                            if (allPumps[key].GetInUse)
                            {
                                allPumps[key].IsInUse = false;

                                if (key != 3 && key != 6 && key != 9)
                                {
                                    if (allPumps[key + 1].GetInUse)
                                    {
                                        allPumps[key + 1].IsInUse = false;

                                        if (key != 2 && key != 5 && key != 8)
                                        {
                                            if (allPumps[key + 2].GetInUse)
                                            {
                                                allPumps[key + 2].IsInUse = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    
                    foreach (Vehicle vehicle in allVehicles.ToList())
                    {
                        int vehiclesPumpNumber = vehicle.pumpNumber;

                        if (vehiclesPumpNumber != 0)
                        {
                            allPumps[vehiclesPumpNumber].IsInUse = true;

                            if (vehiclesPumpNumber != 3 && vehiclesPumpNumber != 6 && vehiclesPumpNumber != 9)
                            {
                                if (!allPumps[vehiclesPumpNumber + 1].GetInUse)
                                {
                                    allPumps[vehiclesPumpNumber + 1].IsInUse = true;

                                    if (vehiclesPumpNumber != 2 && vehiclesPumpNumber != 5 && vehiclesPumpNumber != 8)
                                    {
                                        if (!allPumps[vehiclesPumpNumber + 2].GetInUse)
                                        {
                                            allPumps[vehiclesPumpNumber + 2].IsInUse = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    pumpControl.Start();
                    pumpControl.Wait();

                    break;
            }
        }

        /// <summary>
        /// Create a random vehicle sub-object of Car, Van or Truck
        /// </summary>
        /// <returns></returns>
        private Vehicle CreateVehicle()
        {
            Vehicle vehicle = new Vehicle.Car();

            int randomVehicle = random.Next(0, 3);

            switch (randomVehicle)
            {
                case 0:
                    vehicle = new Vehicle.Car();
                    break;

                case 1:
                    vehicle = new Vehicle.Van();
                    break;

                case 2:
                    vehicle = new Vehicle.Truck();
                    break;
            }

            return vehicle;
        }

        /// <summary>
        /// Delete a vehicle object from the correct lists
        /// </summary>
        /// <param name="state">What list to delete the vehicle object from</param>
        /// <param name="vehicle">The vehicle object to remove</param>
        /// <param name="dispatcher">The CoreDispatcher to update the UI on the correct thread</param>
        private async void DeleteVehicle(string state, Vehicle vehicle, CoreDispatcher dispatcher)
        {
            switch (state)
            {
                case "queue":
                    queuingVehicles.Remove(vehicle);

                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        counterBlocks[0].Text = $"Vehicles Queuing: {queuingVehicles.Count}";
                    });

                    break;

                case "allVeh":
                    numberVehiclesServiced++;

                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        counterBlocks[1].Text = $"Vehicles Serviced: {numberVehiclesServiced}";
                    });

                    allVehicles.Remove(vehicle);
                    break;

                case "both":
                    numberVehiclesLeft++;

                    queuingVehicles.Remove(vehicle);
                    allVehicles.Remove(vehicle);

                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        counterBlocks[0].Text = $"Vehicles Queuing: {queuingVehicles.Count}";
                        counterBlocks[2].Text = $"Vehicle Left: {numberVehiclesLeft}";
                    });

                    break;
            }
        }

        /// <summary>
        /// Start a sub-task for increasing a vehicle's fuel at the same time as the current thread runs
        /// </summary>
        /// <param name="vehicle">Vehicle object to update fuel for</param>
        /// <param name="pumpNumber">The pump number that is being occupied</param>
        /// <param name="progressBar">The ProgressBar that is linked to this pump</param>
        /// <param name="window">The CoreWindow to get the CoreDispatcher to update the UI on the correct thread</param>
        private async void IncreaseFuelForVehicle(Vehicle vehicle, int pumpNumber, ProgressBar progressBar, CoreWindow window)
        {
            try
            {
                Pump pump = allPumps[pumpNumber];

                vehicle.pumpNumber = pumpNumber;

                decimal dispenseRate = pump.GetRate;

                CoreDispatcher dispatcher = window.Dispatcher;

                Task<decimal[]> IncreaseFuel = vehicle.IncreaseFuelAsync(dispenseRate, progressBar, dispatcher);

                IncreaseFuel.Wait();

                if (IncreaseFuel.IsCompleted)
                {
                    decimal[] returnArray = IncreaseFuel.Result;

                    totalMoneyEarned += returnArray[0];
                    totalLitresDispensed += returnArray[1];

                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        counterBlocks[3].Text = $"Total Litres Dispensed: {Math.Round(totalLitresDispensed, 2)}";
                        counterBlocks[4].Text = $"Total Money Earnt: £{Math.Round(totalMoneyEarned, 2)}";
                    });

                    pump.ChangeLitres = returnArray[1];
                    pump.IncreaseVehicles = 1;

                    int vehiclePumpNumber = vehicle.pumpNumber;

                    DeleteVehicle("allVeh", vehicle, dispatcher);

                    ChangePumps("leave", vehiclePumpNumber, dispatcher);
                }
            } 
            catch
            {

            }
            
        }

        /// <summary>
        /// Wait in the queuing vehicles for a random time before removing itself
        /// </summary>
        /// <param name="vehicle">The vehicle object to be dealt with</param>
        /// <param name="waitingTime">The time to wait until being removed if no pump assigned</param>
        /// <param name="dispatcher">The CoreDispatcher to update the UI on the correct thread</param>
        private void WaitInQueue(Vehicle vehicle, int waitingTime, CoreDispatcher dispatcher)
        {
            int seconds = 0;

            while (seconds < (waitingTime / 1000) && loggedIn)
            {
                seconds++;

                if (vehicle.pumpNumber != 0)
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            if (vehicle.pumpNumber == 0)
            {
                numberVehiclesLeft++;
                DeleteVehicle("both", vehicle, dispatcher);

            }
            else
            {
                DeleteVehicle("queue", vehicle, dispatcher);
            }
        }

        /// <summary>
        /// Create all vehicles at a random time constantly through the app's lifetime
        /// </summary>
        /// <param name="cancelToken">The token to cancel this task</param>
        /// <param name="dispatcher">The CoreDispatcher to update the UI on the correct thread</param>
        private async Task VehicleCreation(CancellationToken cancelToken, CoreDispatcher dispatcher, AutoResetEvent restart)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                if (queuingVehicles.Count < 5)
                {
                    Vehicle vehicle = CreateVehicle();

                    queuingVehicles.Add(vehicle);
                    allVehicles.Add(vehicle);

                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        counterBlocks[0].Text = $"Vehicles Queuing: {queuingVehicles.Count}";
                    });

                    _ = Task.Run(() =>
                        {
                            WaitInQueue(vehicle, random.Next(5000, 10000), dispatcher);
                        });
                }

                Thread.Sleep(random.Next(1500, 2200));
            }
        }

        /// <summary>
        /// Begin a background task to update the vehicle's fuel 
        /// as well as control queueingvehicle lists
        /// </summary>
        /// <param name="pumpNumber">Pump number being occupied</param>
        /// <param name="pumpBar">The ProgressBar that is linked to the pumpNumber</param>
        /// <param name="window">The CoreWindow to get the CoreDispatcher to update the UI on the correct thread</param>
        private void StartFuelling(int pumpNumber, ProgressBar pumpBar, CoreWindow window)
        {
            if (queuingVehicles != null && queuingVehicles.Any())
            {
                queuingVehicles.FirstOrDefault().pumpNumber = pumpNumber;

                Task increaseFuelOnVehicle = new Task(() => IncreaseFuelForVehicle(queuingVehicles.FirstOrDefault(), pumpNumber, pumpBar, window));
                increaseFuelOnVehicle.Start();

                DeleteVehicle("queue", queuingVehicles.FirstOrDefault(), window.Dispatcher);
            }
        }

        /// <summary>
        /// Start the Vehicle Creation task to create vehicles 
        /// as well as set up the 9 pumps with a random constructor
        /// </summary>
        /// <param name="dispatcher"></param>
        private void Run(CoreDispatcher dispatcher)
        {
            totalLitresDispensed = 0;
            totalMoneyEarned = 0;
            commission = 0;

            numberVehiclesServiced = 0;
            numberVehiclesLeft = 0;

            allVehicles.Clear();
            queuingVehicles.Clear();

            counterBlocks[0].Text = $"Vehicles Queuing: 0";
            counterBlocks[1].Text = $"Vehicles Serviced: 0";
            counterBlocks[2].Text = $"Vehicle Left: 0";
            counterBlocks[3].Text = $"Total Litres Dispensed: 0";
            counterBlocks[4].Text = $"Total Money Earnt: £0.00";

            CancellationToken cancellation = cancelToken.Token;

            Task.Run(() =>
            {
                _ = VehicleCreation(cancelToken.Token, dispatcher, resetEvent);
            });

            if (allPumps.Count == 0)
            {
                for (int i = 1; i <= 9; i++)
                {
                    int randomNum = random.Next(1, 3);

                    Pump pump = new Pump(0);

                    switch (randomNum)
                    {
                        case 1:
                            pump = new Pump(i);
                            break;

                        case 2:
                            pump = new Pump(i, 3);
                            break;
                    }

                    allPumps.Add(i, pump);
                }
            }
        }

        /// <summary>
        /// Control the "Available" or "Busy" statuses displayed the UI
        /// </summary>
        /// <param name="dispatcher">The CoreDispatcher to update the UI on the correct thread</param>
        private async void ControlPumpText(CoreDispatcher dispatcher)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                foreach (TextBlock pump in pumpStatuses)
                {
                    int accessKey = Convert.ToInt32(pump.AccessKey);

                    if (allPumps[accessKey].GetInUse)
                    {
                        pump.Text = $"Busy";

                    }
                    else
                    {
                        pump.Text = $"Available";
                    }

                }
            });
        }

        /// <summary>
        /// Called when a pump is selected, 
        /// starts the process of starting the vehicle fuelling
        /// </summary>
        /// <param name="accessKey">The accessKey of the pump in question (Used to get the pumpNumber)</param>
        public void SelectPump(string accessKey)
        {
            int pumpNumber = Convert.ToInt32(accessKey);

            if (!allPumps[pumpNumber].GetInUse)
            {
                if ((queuingVehicles != null) && (queuingVehicles.Any())) {
                    Task changePumps = new Task(() => { ChangePumps("add", Convert.ToInt32(pumpNumber), window.Dispatcher); });

                    changePumps.Start();
                    changePumps.Wait();

                    StartFuelling(pumpNumber, pumpBars[pumpNumber - 1], window);
                }
            }
        }

        /// <summary>
        /// Called when the logout button is clicked
        /// </summary>
        /// <param name="PumpGrid">The Grid that holds all pumps</param>
        /// <param name="CounterGrid">The Grid that holds all the counters</param>
        /// <param name="LogoutGrid">The Grid that holds all logout information (Overview grid)</param>
        /// <param name="LogoutButton">The Logout Button that will be hidden in this method</param>
        /// <param name="logoutText">The list of all textblocks for logout overview information</param>
        public async void Logout(Grid PumpGrid, Grid CounterGrid, Grid LogoutGrid, Button LogoutButton, List<TextBlock> logoutText)
        {
            commission = (decimal) (8 * 5.9) + Math.Round(totalMoneyEarned * 0.01M, 2);
            totalLitresDispensed = Math.Round(totalLitresDispensed, 2);
            totalMoneyEarned = Math.Round(totalMoneyEarned, 2);

            logoutText[0].Text = $"Vehicles Serviced: {numberVehiclesServiced}";
            logoutText[1].Text = $"Vehicles Left: {numberVehiclesLeft}";
            logoutText[2].Text = $"Total Litres dispensed: {totalLitresDispensed}";
            logoutText[3].Text = $"Total Money Earnt: £{totalMoneyEarned}";
            logoutText[4].Text = $"Commission: £{commission}";

            PumpGrid.Visibility = Visibility.Collapsed;
            CounterGrid.Visibility = Visibility.Collapsed;
            LogoutButton.Visibility = Visibility.Collapsed;

            LogoutGrid.Visibility = Visibility.Visible;

            await ApplicationData.Current.LocalFolder.CreateFileAsync("Database.db", CreationCollisionOption.OpenIfExists);

            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection connection = new SqliteConnection($"Data Source={path}"))
            {
                connection.Open();

                SqliteCommand sqliteCommand = connection.CreateCommand();

                sqliteCommand.CommandText = @"INSERT INTO Log (attendantId, numberVehiclesServiced, numberVehiclesLeft, totalLitresDispensed, totalMoneyEarnt, commission)
                                            VALUES (@attendantId, @numberVehiclesServiced, @numberVehiclesLeft, @totalLitresDispensed, @totalMoneyEarnt, @commission)";

                sqliteCommand.Parameters.AddWithValue("@attendantId", attendantId);
                sqliteCommand.Parameters.AddWithValue("@numberVehiclesServiced", numberVehiclesServiced);
                sqliteCommand.Parameters.AddWithValue("@numberVehiclesLeft", numberVehiclesLeft);
                sqliteCommand.Parameters.AddWithValue("@totalLitresDispensed", totalLitresDispensed);
                sqliteCommand.Parameters.AddWithValue("@totalMoneyEarnt", totalMoneyEarned);
                sqliteCommand.Parameters.AddWithValue("@commission", commission);


                sqliteCommand.ExecuteReader();

                connection.Close();
            }
        }

        /// <summary>
        /// Opens a specific pump's information, 
        /// displaying dispense rate, 
        /// num of vehicles serviced 
        /// as well as total litres dispensed by the pump
        /// </summary>
        /// <param name="accessKey">Access key to get the correct pump number</param>
        /// <param name="pumpInfoGrid">Grid containing all the pump information </param>
        /// <param name="pumpInfo">All textblocks within the pumpInfoGrid Grid</param>
        public void ViewPumpInfo(string accessKey, Grid pumpInfoGrid, List<TextBlock> pumpInfo)
        {
            int pumpNumber = Convert.ToInt32(accessKey);
            Pump pump = allPumps[pumpNumber];

            pumpInfo[0].Text = $"Pump Number {pumpNumber}";
            pumpInfo[1].Text = $"Vehicles Serviced: {pump.GetVehicles}";
            pumpInfo[2].Text = $"Dispension Rate (l/s): {pump.GetRate}";
            pumpInfo[3].Text = $"Litres Dispensed: {pump.GetLitres}";

            pumpInfoGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Called when the login button is pressed
        /// </summary>
        /// <param name="loginDetail">The attendant's ID within the text box</param>
        /// <param name="userWelcome">"Logged in as {username}" in top left</param>
        /// <param name="LoginGrid">Grid containing all login objects</param>
        /// <param name="PumpGrid">Grid containg all pump objects</param>
        /// <param name="CounterGrid">Grid containing all counter objects</param>
        /// <param name="LogoutButton">Button to logout from</param>
        public async void Login(int loginDetail, TextBlock userWelcome, Grid LoginGrid, Grid PumpGrid, Grid CounterGrid, Button LogoutButton)
        {
            resetEvent.Set();

            loggedIn = false;

            await ApplicationData.Current.LocalFolder.CreateFileAsync("Database.db", CreationCollisionOption.OpenIfExists);

            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection connection = new SqliteConnection($"Data Source={path}"))
            {
                connection.Open();

                SqliteCommand command = connection.CreateCommand();

                command.CommandText = $"SELECT * FROM Attendant WHERE attendantId = {loginDetail}";

                using (SqliteDataReader databaseReader = command.ExecuteReader())
                {
                    while (databaseReader.Read())
                    {
                        string dateJoined = databaseReader.GetString(3);

                        DateTime date = DateTime.Parse(dateJoined);
                        DateTime dateToday = DateTime.Now;

                        attendantId = databaseReader.GetInt32(0);

                        userWelcome.Text = $"Logged in as {databaseReader.GetString(1)} {databaseReader.GetString(2)}";

                        loggedIn = true;
                    }
                }

                connection.Close();
            }

            if (loggedIn)
            {                
                PumpGrid.Visibility = Visibility.Visible;
                CounterGrid.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Visible;
                userWelcome.Visibility = Visibility.Visible;

                LoginGrid.Visibility = Visibility.Collapsed;

                CoreDispatcher dispatcher = window.Dispatcher;

                Run(dispatcher);
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("Wrong")
                {
                    Title = "Attendant ID not found!",
                    Content = "The ID you entered was not found in the database"
                };

                await messageDialog.ShowAsync();
            }
        }

        /// <summary>
        /// Called when the app is initially run, 
        /// copying the attached database to the correct LocalFolder,
        /// setting up lists to be accessed constantly from this file
        /// </summary>
        /// <param name="progressBars">List of all ProgressBar objects to be accessed</param>
        /// <param name="textBlocks">List of all pumpStatus objects to be accessed</param>
        /// <param name="counters">List of all counter objects to be accessed</param>
        public async void Initialise(List<ProgressBar> progressBars, List<TextBlock> textBlocks, List<TextBlock> counters)
        {
            pumpBars = progressBars;
            pumpStatuses = textBlocks;
            counterBlocks = counters;

            StorageFolder localStorage = ApplicationData.Current.LocalFolder;
            StorageFolder installLocation = Package.Current.InstalledLocation;

            StorageFile storageFile = await installLocation.GetFileAsync("Database.db");

            if (await localStorage.TryGetItemAsync("Database.db") == null)
            {
                await storageFile.CopyAsync(localStorage);
            }
        }
    }
}
