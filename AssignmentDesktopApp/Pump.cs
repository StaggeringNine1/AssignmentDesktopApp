using System;

namespace AssignmentDesktopApp
{
    public class Pump
    {
        /// <summary>
        /// Receive pump number
        /// </summary>
        public int GetNum { get; private set; }

        /// <summary>
        /// Receive the current status of the pump, free or in use
        /// </summary>
        public bool GetInUse { get; private set; }

        /// <summary>
        /// Receive the amount of litres this pump has dispensed in the lifetime
        /// </summary>
        public decimal GetLitres { get; private set; }

        /// <summary>
        /// Receive the litres per second this pump can provide
        /// </summary>
        public decimal GetRate { get; private set; }

        /// <summary>
        /// Receive the number of vehicles serviced by this pump
        /// </summary>
        public int GetVehicles { get; private set; }

        /// <summary>
        /// Change the pump number of this value. Usage: ChangeNum = int
        /// </summary>
        private int ChangeNum
        {
            set { GetNum = value; }
        }

        /// <summary>
        /// Change the dispense rate of this value. Usage: ChangeRate = double
        /// </summary>
        private decimal ChangeRate
        {
            set { GetRate = value; }
        }

        /// <summary>
        /// Increment the number of vehicles serviced by one. Usage: GetVehicles = 1
        /// </summary>
        public int IncreaseVehicles
        {
            set { GetVehicles += 1; }
            get { return GetVehicles; }
        }

        /// <summary>
        /// Change the current status of the pump. Usage: ChangeStatus = bool
        /// </summary>
        public bool IsInUse
        {
            set { GetInUse = value; }
        }

        /// <summary>
        /// Increase the amount of litres this pump has dispensed. Usage: ChangeLitres = double;
        /// </summary>
        public decimal ChangeLitres
        {
            set { GetLitres += value; }
        }

        /// <summary>
        /// Create a new pump, using default dispense rate
        /// </summary>
        /// <param name="pumpNumber">The pump number of this pump</param>
        public Pump (int pumpNumber)
        {
            if (pumpNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("pumpNumber");
            }

            ChangeNum = pumpNumber;
            ChangeRate = 1.5M;
            ChangeLitres = 0;
            GetInUse = false;
        }

        /// <summary>
        /// Create a new pump, using custom dispense rate
        /// </summary>
        /// <param name="pumpNumber">The pump number of this pump</param>
        /// <param name="dispensionRate">The dispension rate of this pump in litres per second</param>
        public Pump (int pumpNumber, decimal dispensionRate)
        {
            if (pumpNumber <= 0)
            {
                throw new ArgumentOutOfRangeException("pumpNumber");
            }

            if (dispensionRate <= 0M)
            {
                throw new ArgumentOutOfRangeException("dispensionRate");
            }

            ChangeNum = pumpNumber;
            ChangeRate = dispensionRate;
            ChangeLitres = 0;
            GetInUse = false;
        }        

    }
}
