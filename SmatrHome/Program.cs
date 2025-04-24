using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SmartHomeSystem
{
    public abstract class SmartDevice
    {
        public string Name { get; set; }
        public double EnergyConsumption { get; set; }
        public bool IsOn { get; private set; }

        public SmartDevice(string name, double energyConsumption)
        {
            Name = name;
            EnergyConsumption = energyConsumption;
        }

        public void TurnOn()
        {
            IsOn = true;
            Console.WriteLine($"{Name} is on.");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"{Name} is off.");
        }

        public abstract void DisplayStatus();
    }
    public class SmartLight : SmartDevice
    {
        public int Brightness { get; set; }
        public string Color { get; set; }

        public SmartLight(string name, double energyConsumption, int brightness, string color)
            : base(name, energyConsumption)
        {
            Brightness = brightness;
            Color = color;
        }

        public override void DisplayStatus()
        {
            Console.WriteLine($"SmartLight: {Name}\n Brightness: {Brightness}\n Color: {Color}\n Status: {(IsOn ? "On" : "Off")}");
        }
    }
    public class SmartThermostat : SmartDevice
    {
        public double CurrentTemperature { get; set; }
        public double DesiredTemperature { get; set; }

        public SmartThermostat(string name, double energyConsumption, double currentTemp, double desiredTemp)
            : base(name, energyConsumption)
        {
            CurrentTemperature = currentTemp;
            DesiredTemperature = desiredTemp;
        }

        public override void DisplayStatus()
        {
            Console.WriteLine($"SmartThermostat: {Name}\n Current Temperature: {CurrentTemperature}\n Desired Temperature: {DesiredTemperature}, Status: {(IsOn ? "On" : "Off")}");
        }
    }

    public class SmartHome : IEnumerable<SmartDevice> // Итератор чрез интерфейс IEnumerable
    {
        private List<SmartDevice> devices = new List<SmartDevice>();

        public void AddDevice(SmartDevice device)
        {
            devices.Add(device);
        }

        // итератор за обхождане на устройствата
        public IEnumerator<SmartDevice> GetEnumerator()
        {
            foreach (var device in devices)
            {
                yield return device;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Извличане на свойства и методи чрез Reflection
        public void DisplayDeviceInfoUsingReflection()
        {
            foreach (var device in devices)
            {
                Type type = device.GetType();
                Console.WriteLine($"Information for: {type.Name}");

                Console.WriteLine("Properties:");
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    Console.WriteLine($" - {prop.Name} : {prop.PropertyType.Name}");
                }

                Console.WriteLine("Methods:");
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    Console.WriteLine($" - {method.Name}");
                }
            }
        }
    }

    // Сравняване по енергийна консумация чрез компаратор
    public class EnergyComparer : IComparer<SmartDevice>
    {
        public int Compare(SmartDevice x, SmartDevice y)
        {
            return x.EnergyConsumption.CompareTo(y.EnergyConsumption);
        }
    }

    //Компаратор за сравняване по име
    public class NameComparer : IComparer<SmartDevice>
    {
        public int Compare(SmartDevice x, SmartDevice y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
    class Program
    {
        static void Main()
        {
            SmartLight light1 = new SmartLight("Lamp", 10.5, 75, "White");
            SmartLight light2 = new SmartLight("Lamp2", 9.2, 100, "Green");
            SmartThermostat thermostat1 = new SmartThermostat("Thermostat", 15.8, 20.0, 22.5);

            SmartHome home = new SmartHome();
            home.AddDevice(light1);
            home.AddDevice(light2);
            home.AddDevice(thermostat1);

            light1.TurnOn();
            thermostat1.TurnOn();

            Console.WriteLine("\n-- All properties --");
            foreach (var device in home) 
            {
                device.DisplayStatus();
            }

            Console.WriteLine("\n-- Sorted by energy --");
            // Сортиране по енергия с EnergyComparer
            List<SmartDevice> sortedByEnergy = new List<SmartDevice>(home);
            sortedByEnergy.Sort(new EnergyComparer());
            foreach (var device in sortedByEnergy)
            {
                device.DisplayStatus();
            }

            Console.WriteLine("\n-- Sorted by name --");
            //Сортиране по име с NameComparer
            List<SmartDevice> sortedByName = new List<SmartDevice>(home);
            sortedByName.Sort(new NameComparer());
            foreach (var device in sortedByName)
            {
                device.DisplayStatus();
            }

            Console.WriteLine("\n-- Reflection: Information for the device --");
            home.DisplayDeviceInfoUsingReflection();
        }
    }
}
