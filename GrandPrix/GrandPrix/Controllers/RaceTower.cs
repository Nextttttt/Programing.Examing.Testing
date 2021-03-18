﻿namespace GrandPrix.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Models.Drivers;
    using Factories;
    using System.Linq;

    class RaceTower
    {
        private DriverFactory driverFactory;
        private TyreFactory tyreFactory;
        
        private List<Driver> drivers = new List<Driver>();
        private List<Driver> failedDrivers = new List<Driver>();

        private StringBuilder stringBuilder;
        private int currentLap = 1;
        private int totalLaps = 0;
        private int trackLength = 0;
        private string currentWeather = "Sunny";

        public RaceTower()
        {
            this.driverFactory = new DriverFactory();
            this.drivers = new List<Driver>();
            this.stringBuilder = new StringBuilder();
            totalLaps = 0;
        }

        private const string GREATER_LAPS_ERROR_MASSAGE = "There is no time!";

        public void SetTrackInfo(int lapsNumber, int trackLength)
        {

            this.totalLaps = lapsNumber;
            this.trackLength = trackLength;

        }
        public void RegisterDriver(List<string> commandArgs)
        {
            this.stringBuilder.Clear();

            try
            {
                Driver driver = driverFactory.Create(commandArgs);
                this.drivers.Add(driver);
            }
            catch (Exception ex)
            {
                this.stringBuilder.Append(ex.Message);
            }
            
        }

        public void DriverBoxes(List<string> commandArgs)
        {
            //TODO: Add some logic here …
            string reasonToBox = commandArgs[0];
            string driverName  = commandArgs[1];
            

            switch(reasonToBox)
            {
                case "ChangeTyres":
                    {
                        string tyreType = commandArgs[2];
                        double tyreHardness = double.Parse(commandArgs[3]);
                        double grip = double.Parse(commandArgs[4]);

                        foreach (var item in drivers.Where(x => x.Name == driverName))
                        {
                            item.Car.Tyre = tyreFactory.Create(tyreType, tyreHardness, grip);
                            item.TotalTime += 20;
                        }

                        break;
                    }
                case "Refuel":
                    {
                        double fuelAmount = double.Parse(commandArgs[2]);

                        foreach (var item in drivers.Where(x => x.Name == driverName))
                        {
                            item.Car.FuelAmount += fuelAmount;
                            item.TotalTime += 20;
                        }

                        break;
                    }
            }
        }

        public string CompleteLaps(List<string> commandArgs)
        {
            //TODO: Add some logic here …
            string result = "";
            this.stringBuilder.Clear();
            int numberOfLaps = int.Parse(commandArgs[0]);
            int counter = 0;

            List<Driver> Sorted = drivers;
            Sorted.OrderBy(d => d.TotalTime);

            Driver firstDriver = drivers[0];
            currentLap += numberOfLaps;

            if(currentLap == totalLaps)
            {
                result = this.stringBuilder.AppendLine($"{drivers[0].Name} wins the race for {drivers[0].TotalTime} seconds.").ToString();
                return result;
            }

            try
            {
                if(currentLap > totalLaps)
                {
                    throw new Exception(GREATER_LAPS_ERROR_MASSAGE);
                }
            }
            catch (Exception ex)
            {

                result = this.stringBuilder.AppendLine(ex.Message).ToString();
                return result;

            }
            

            foreach (var item in drivers)
            {
                item.TotalTime += (60 / trackLength / item.Speed) * numberOfLaps;
                item.Car.FuelAmount -= trackLength * item.FuelConsumptionPerKm;
                try
                {
                    item.Car.Tyre.Degradation -= item.Car.Tyre.Hardness;
                }
                catch(Exception ex)
                {
                    item.failureReason = ex.Message;
                    failedDrivers.Add(item);
                    drivers.Remove(item);
                }
                
                counter++;
            }

            for(int i = 0; i < counter; i ++)
            {

                if(Sorted[i].TotalTime < Sorted[i + 1].TotalTime && Sorted[i].overtaked == false)
                {
                    switch(Sorted[i].Type)
                    {
                        case "Agressive":
                            {

                                if(Sorted[i + 1].TotalTime - Sorted[i].TotalTime <= 3 && Sorted[i].Car.Tyre.Type == "Ultrasoft")
                                {
                                    if (currentWeather == "Foggy")
                                    {
                                        Sorted[i].failureReason = "Crashed";
                                        failedDrivers.Add(Sorted[i]);
                                        Sorted.Remove(Sorted[i]);

                                        break;
                                    }
                                    Sorted[i].overtaked = true;

                                    var buffer = Sorted[i];
                                    Sorted[i] = Sorted[i + 1];
                                    Sorted[i + 1] = buffer;

                                    Sorted[i].TotalTime += 3;
                                    Sorted[i + 1].TotalTime -= 3;
                                    Sorted[i].overtaked = false;
                                }
                                else if(Sorted[i + 1].TotalTime - Sorted[i].TotalTime <= 2)
                                {
                                    if (currentWeather == "Foggy")
                                    {
                                        Sorted[i].failureReason = "Crashed";
                                        failedDrivers.Add(Sorted[i]);
                                        Sorted.Remove(Sorted[i]);

                                        break;
                                    }
                                    Sorted[i].overtaked = true;

                                    var buffer = Sorted[i];
                                    Sorted[i] = Sorted[i + 1];
                                    Sorted[i + 1] = buffer;

                                    Sorted[i].TotalTime += 2;
                                    Sorted[i + 1].TotalTime -= 2;
                                    Sorted[i].overtaked = false;
                                }
                                result = stringBuilder.AppendLine($"{Sorted[i+1].Name} has overtaken {Sorted[i]} on lap {currentLap}.").ToString();

                                break;
                            }
                        case "Endurance":
                            {
                                

                                if (Sorted[i + 1].TotalTime - Sorted[i].TotalTime <= 3 && Sorted[i].Car.Tyre.Type == "Hard")
                                {
                                    if (currentWeather == "Rainy")
                                    {
                                        Sorted[i].failureReason = "Crashed";
                                        failedDrivers.Add(Sorted[i]);
                                        Sorted.Remove(Sorted[i]);

                                        break;
                                    }
                                    Sorted[i].overtaked = true;

                                    var buffer = Sorted[i];
                                    Sorted[i] = Sorted[i + 1];
                                    Sorted[i + 1] = buffer;

                                    Sorted[i].TotalTime += 3;
                                    Sorted[i + 1].TotalTime -= 3;
                                    Sorted[i].overtaked = false;
                                }
                                else if(Sorted[i + 1].TotalTime - Sorted[i].TotalTime <= 2)
                                {
                                    if (currentWeather == "Rainy")
                                    {
                                        Sorted[i].failureReason = "Crashed";
                                        failedDrivers.Add(Sorted[i]);
                                        Sorted.Remove(Sorted[i]);

                                        break;
                                    }
                                    Sorted[i].overtaked = true;

                                    var buffer = Sorted[i];
                                    Sorted[i] = Sorted[i + 1];
                                    Sorted[i + 1] = buffer;

                                    Sorted[i].TotalTime += 2;
                                    Sorted[i + 1].TotalTime -= 2;
                                    Sorted[i].overtaked = false;
                                }

                                result = stringBuilder.AppendLine($"{Sorted[i + 1].Name} has overtaken {Sorted[i]} on lap {currentLap}.").ToString();

                                break;
                            }
                    }
                    

                    
                }

            }

            return result;
        }

        public string GetLeaderboard()
        {

            string result;
            int position = 0;

            drivers.OrderByDescending(d => d.TotalTime);

            //TODO: Add some logic here …
            this.stringBuilder.Clear();
            this.stringBuilder.AppendLine($"Lap {currentLap}/{totalLaps}");
            foreach(var item in drivers)
            {
                position++;
                this.stringBuilder.AppendLine($"{position} {item.Name} {item.TotalTime}");
            }

            result = this.stringBuilder.ToString();
            return result;
        }
        
        void ChangeWeather(List<string> commandArgs)
        {
            //TODO: Add some logic here …
            string weather = commandArgs[0];
            currentWeather = weather;
        }

    }
}
