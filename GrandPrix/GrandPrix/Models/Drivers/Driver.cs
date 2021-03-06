﻿
namespace GrandPrix.Models.Drivers
{
    using GrandPrix.Models.Tyres;
    using GrandPrix.Factories;
    public abstract class Driver : BaseModel
    {

        private double speed;
        public bool overtaked = false;
        public string failureReason = null;
        

        public Driver(string name,int hp,double fuelAmount, string tyreType, double tyreHardness, double grip)
            :base(name)
        {

            this.Car = new Car(hp, fuelAmount, tyreType, tyreHardness, grip);
            this.Speed = (Car.HP + Car.Tyre.Degradation) / Car.FuelAmount;

        }
        public Driver(string name, int hp, double fuelAmount, string tyreType, double tyreHardness)
            : base(name)
        {

            this.Car = new Car(hp, fuelAmount, tyreType, tyreHardness);
            this.Speed = (Car.HP + Car.Tyre.Degradation) / Car.FuelAmount;

        }

        public double TotalTime { get; set; }
        public Car Car { get; set; }
        public double FuelConsumptionPerKm { get; set; }
        public double Speed { get; set; }


    }
}
