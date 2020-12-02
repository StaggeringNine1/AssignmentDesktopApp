using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

namespace AssignmentDesktopApp
{
    public abstract class Vehicle
    {
        private readonly decimal MaxFuelLitres;
        private decimal CurrentFuelLitres;
        private decimal litresDispensed;
        private decimal totalCost;
        private string typeOfFuel;

        public int pumpNumber = 0;

        static readonly Random random = new Random();

        private List<string> FuelTypes;

        /// <summary>
        /// Abstract class containing all main information for the three inherited classes (Car, Van, Truck),
        /// Randomly generates fuel capacity from a quarter of the max fuel tank
        /// </summary>
        /// <param name="MaxFuelLitres">Controlled within the class itself, cannot be assigned to</param>
        public Vehicle(int MaxFuelLitres)
        {
            this.MaxFuelLitres = MaxFuelLitres;
            CurrentFuelLitres = Convert.ToDecimal((random.NextDouble() * (MaxFuelLitres / 4)).ToString("N2"));
        }

        public class Car : Vehicle
        {
            /// <summary>
            /// Creates a new car vehicle, with 50 max litres and a random fuel type of Petrol, LPG or Diesel
            /// </summary>
            public Car() : base(50)
            {
                FuelTypes = new List<string>() { "Petrol", "LPG", "Diesel" };
                typeOfFuel = FuelTypes[random.Next(0, FuelTypes.Count)];
            }
        }

        public class Van : Vehicle
        {
            /// <summary>
            /// Creates a new van vehicle, with 80 max litres and random fuel type of LPG or Diesel
            /// </summary>
            public Van() : base(80)
            {
                FuelTypes = new List<string>() { "LPG", "Diesel" };
                typeOfFuel = FuelTypes[random.Next(0, FuelTypes.Count)];
            }
        }

        public class Truck : Vehicle
        {
            /// <summary>
            /// Creates a new truck vehicle, with 150 max litres and only Diesel fuel type
            /// </summary>
            public Truck() : base(150)
            {
                FuelTypes = new List<string>() { "Diesel" };
                typeOfFuel = FuelTypes[random.Next(0, FuelTypes.Count)];
            }
        }

        /// <summary>
        /// Starts the fuelling process of the vehicle
        /// </summary>
        /// <param name="dispenseRate">The amount of litres the tank fills up per second</param>
        /// <param name="progressBar">Progress bar on UI to show this progress through percentages</param>
        /// <param name="dispatcher">The CoreDispatcher to update the progress bar on the correct thread</param>
        /// <returns></returns>
        public async Task<decimal[]> IncreaseFuelAsync(decimal dispenseRate, ProgressBar progressBar, CoreDispatcher dispatcher)
        {
            int secondsPassed = 0;
            double totalFuelTime = random.Next(15000, 20000);

            double percent = 0;

            if (progressBar == null) throw new ArgumentNullException();

            do
            {
                if (CurrentFuelLitres < MaxFuelLitres)
                {
                    CurrentFuelLitres += dispenseRate;
                    litresDispensed += dispenseRate;

                }
                else
                {
                    CurrentFuelLitres = MaxFuelLitres;
                    litresDispensed += MaxFuelLitres - CurrentFuelLitres;
                    break;
                }

                percent = (secondsPassed / (totalFuelTime / 1000)) * 100;

                secondsPassed++;

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                 {
                     progressBar.Value = percent;
                 });

                Thread.Sleep(1000);

            } while (secondsPassed < (totalFuelTime / 1000));

            switch (typeOfFuel)
            {
                case ("Petrol"):
                    totalCost = litresDispensed * 1.15M;
                    break;

                case ("LPG"):
                    totalCost = litresDispensed * 0.63M;
                    break;

                case ("Diesel"):
                    totalCost = litresDispensed * 1.18M;
                    break;
            }

            decimal[] returnValues = new decimal[2];

            returnValues[0] = totalCost;
            returnValues[1] = litresDispensed;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                progressBar.Value = 0;
            });

            return returnValues;
        }
    }
}
