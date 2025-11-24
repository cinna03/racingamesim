using System;

namespace TimeBasedRacingGame.Domain
{
    /// <summary>
    /// Represents different vehicle types with unique characteristics
    /// </summary>
    public enum VehicleType
    {
        /// <summary>Sports vehicle with high speed and moderate fuel consumption</summary>
        SportsVehicle,
        /// <summary>Eco-friendly vehicle with low fuel consumption</summary>
        EcoVehicle,
        /// <summary>High-performance racing vehicle with maximum speed</summary>
        RacingVehicle
    }

    /// <summary>
    /// Represents a racing vehicle with fuel, speed, and performance characteristics
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// Gets the vehicle type
        /// </summary>
        public VehicleType Type { get; }

        /// <summary>
        /// Gets the vehicle name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the maximum speed of the vehicle
        /// </summary>
        public int MaxSpeed { get; }

        /// <summary>
        /// Gets the fuel consumption rate per action
        /// </summary>
        public double FuelConsumption { get; }

        /// <summary>
        /// Gets the maximum fuel capacity
        /// </summary>
        public double MaxFuel { get; }

        /// <summary>
        /// Gets or sets the current fuel level
        /// </summary>
        public double CurrentFuel { get; set; }

        /// <summary>
        /// Gets or sets the current speed
        /// </summary>
        public int CurrentSpeed { get; set; }

        /// <summary>
        /// Initializes a new instance of the Vehicle class
        /// </summary>
        /// <param name="type">The vehicle type</param>
        /// <param name="name">The vehicle name</param>
        /// <param name="maxSpeed">Maximum speed</param>
        /// <param name="fuelConsumption">Fuel consumption rate</param>
        /// <param name="maxFuel">Maximum fuel capacity</param>
        public Vehicle(VehicleType type, string name, int maxSpeed, double fuelConsumption, double maxFuel)
        {
            Type = type;
            Name = name;
            MaxSpeed = maxSpeed;
            FuelConsumption = fuelConsumption;
            MaxFuel = maxFuel;
            CurrentFuel = maxFuel;
            CurrentSpeed = 0;
        }

        /// <summary>
        /// Consumes fuel based on current speed and action
        /// </summary>
        /// <param name="speedMultiplier">Speed multiplier for fuel consumption</param>
        /// <exception cref="InvalidOperationException">Thrown when there's insufficient fuel</exception>
        public void ConsumeFuel(double speedMultiplier = 1.0)
        {
            double consumption = FuelConsumption * speedMultiplier;
            if (CurrentFuel < consumption)
                throw new InvalidOperationException("Insufficient fuel for this action");
            
            CurrentFuel = Math.Max(0, CurrentFuel - consumption);
        }

        /// <summary>
        /// Refuels the vehicle to maximum capacity
        /// </summary>
        public void Refuel()
        {
            CurrentFuel = MaxFuel;
        }

        /// <summary>
        /// Gets the fuel percentage remaining
        /// </summary>
        /// <returns>Fuel percentage (0-100)</returns>
        public double GetFuelPercentage()
        {
            return (CurrentFuel / MaxFuel) * 100;
        }

        /// <summary>
        /// Gets detailed vehicle information for display
        /// </summary>
        /// <returns>Formatted vehicle information string</returns>
        public string GetVehicleInfo()
        {
            string strategy = Type switch
            {
                VehicleType.SportsVehicle => "⚡ BALANCED PERFORMANCE - Good speed with moderate fuel efficiency",
                VehicleType.EcoVehicle => "🌱 ECO STRATEGY - Lower speed but excellent fuel economy and high capacity", 
                VehicleType.RacingVehicle => "🏁 SPEED DEMON - Maximum speed but high fuel consumption and low capacity",
                _ => "Unknown vehicle type"
            };
            
            return $"🏎️ {Name}\n" +
                   $"Max Speed: {MaxSpeed} km/h | Fuel Capacity: {MaxFuel}L | Consumption: {FuelConsumption}L/action\n" +
                   $"{strategy}";
        }
    }
}

