using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingSystem
{
    public class ParkingLot
    {
        private readonly int totalSlots;
        private readonly List<ParkingSlot?> slots;

        public ParkingLot(int totalSlots)
        {
            this.totalSlots = totalSlots;
            slots = new List<ParkingSlot?>(totalSlots);
            for (int i = 0; i < totalSlots; i++)
            {
                slots.Add(null);
            }
        }

        public int TotalSlots => totalSlots;

        public int AvailableSlots => slots.Count(slot => slot == null);

        public bool IsFull => AvailableSlots == 0;

        public int ParkVehicle(string registrationNumber, string color, string vehicleType)
        {
            if (IsFull)
            {
                return -1; // Parking lot is full
            }

            int availableSlotNumber = slots.FindIndex(slot => slot == null);
            slots[availableSlotNumber] = new ParkingSlot(registrationNumber, color, vehicleType);
            return availableSlotNumber + 1; // Slot numbers are 1-based
        }

        public bool Leave(int slotNumber)
        {
            if (slotNumber < 1 || slotNumber > totalSlots)
            {
                return false; // Invalid slot number
            }

            slots[slotNumber - 1] = null;
            return true;
        }

        public List<ParkingSlot?> GetOccupiedSlots()
        {
            return slots.Where(slot => slot != null).ToList();
        }

        public List<string> GetRegistrationNumbersByType(string vehicleType)
        {
            return slots
                .Where(slot => slot != null && slot.VehicleType.Equals(vehicleType, StringComparison.OrdinalIgnoreCase))
                .Select(slot => slot.RegistrationNumber)
                .ToList();
        }

        public List<string> GetRegistrationNumbersByColor(string color)
        {
            return slots
                .Where(slot => slot != null && slot.Color.Equals(color, StringComparison.OrdinalIgnoreCase))
                .Select(slot => slot.RegistrationNumber)
                .ToList();
        }

        public int GetSlotNumberByRegistrationNumber(string registrationNumber)
        {
            var slot = slots.FirstOrDefault(slot => slot != null && slot.RegistrationNumber.Equals(registrationNumber, StringComparison.OrdinalIgnoreCase));
            return slot != null ? slots.IndexOf(slot) + 1 : -1; // Slot numbers are 1-based
        }
    }

    public class ParkingSlot
    {
        public ParkingSlot(string registrationNumber, string color, string vehicleType)
        {
            RegistrationNumber = registrationNumber;
            Color = color;
            VehicleType = vehicleType;
        }

        public string RegistrationNumber { get; }
        public string Color { get; }
        public string VehicleType { get; }
    }

    public class Program
    {
        static ParkingLot? parkingLot;

        static void Main()
        {
            Console.WriteLine("Welcome to the Parking System!");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                string[] args = input.Split(' ');

                if (args.Length < 1)
                {
                    Console.WriteLine("Invalid command. Please try again.");
                    continue;
                }

                string command = args[0].ToLower();

                switch (command)
                {
                    case "create_parking_lot":
                        if (args.Length == 2 && int.TryParse(args[1], out int totalSlots))
                        {
                            parkingLot = new ParkingLot(totalSlots);
                            Console.WriteLine($"Created a parking lot with {totalSlots} slots.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Usage: create_parking_lot [total_slots]");
                        }
                        break;

                    case "park":
                        if (parkingLot != null && args.Length == 4)
                        {
                            string registrationNumber = args[1];
                            string color = args[2];
                            string vehicleType = args[3];
                            int availableSlotNumber = parkingLot.ParkVehicle(registrationNumber, color, vehicleType);

                            if (availableSlotNumber > 0)
                            {
                                Console.WriteLine($"Allocated slot number: {availableSlotNumber}");
                            }
                            else
                            {
                                Console.WriteLine("Sorry, parking lot is full");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Usage: park [registration_number] [vehicle_type] [color]");
                        }
                        break;

                    case "leave":
                        if (parkingLot != null && args.Length == 2 && int.TryParse(args[1], out int slotNum))
                        {
                            if (parkingLot.Leave(slotNum))
                            {
                                Console.WriteLine($"Slot number {slotNum} is free");
                            }
                            else
                            {
                                Console.WriteLine("Invalid slot number. Please try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Usage: leave [slot_number]");
                        }
                        break;

                    case "status":
                        if (parkingLot != null)
                        {
                            Console.WriteLine("Slot\tNo.\t\tType\tRegistration No\tColour");
                            var occupiedSlots = parkingLot.GetOccupiedSlots();
                            foreach (var slot in occupiedSlots)
                            {
                                if (slot != null)
                                {
                                    Console.WriteLine($"{occupiedSlots.IndexOf(slot) + 1}\t{slot.RegistrationNumber}\t{slot.VehicleType}\t{slot.Color}");
                                }
                                else
                                {
                                    Console.WriteLine("Slot is null"); // Atau tindakan lain yang sesuai
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Parking lot has not been created yet.");
                        }
                        break;

                    case "type_of_vehicles":
                        if (parkingLot != null && args.Length == 2)
                        {
                            string vehicleType = args[1];
                            var registrationNumbers = parkingLot.GetRegistrationNumbersByType(vehicleType);
                            Console.WriteLine(registrationNumbers.Count);
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Usage: type_of_vehicles [vehicle_type]");
                        }
                        break;

                    case "registration_numbers_for_vehicles_with_odd_plate":
                        if (parkingLot != null)
                        {
                            var oddPlateRegistrationNumbers = parkingLot.GetRegistrationNumbersByType("Motor")
                                .Where(registrationNumber => registrationNumber.Last() % 2 != 0)
                                .ToList();
                            Console.WriteLine(string.Join(", ", oddPlateRegistrationNumbers));
                        }
                        else
                        {
                            Console.WriteLine("Parking lot has not been created yet.");
                        }
                        break;

                    case "registration_numbers_for_vehicles_with_even_plate":
                        if (parkingLot != null)
                        {
                            var evenPlateRegistrationNumbers = parkingLot.GetRegistrationNumbersByType("Motor")
                                .Where(registrationNumber => registrationNumber.Last() % 2 == 0)
                                .ToList();
                            Console.WriteLine(string.Join(", ", evenPlateRegistrationNumbers));
                        }
                        else
                        {
                            Console.WriteLine("Parking lot has not been created yet.");
                        }
                        break;

                    case "registration_numbers_for_vehicles_with_color":
                        if (parkingLot != null && args.Length == 2)
                        {
                            string color = args[1];
                            var registrationNumbers = parkingLot.GetRegistrationNumbersByColor(color);
                            Console.WriteLine(string.Join(", ", registrationNumbers));
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Usage: registration_numbers_for_vehicles_with_color [color]");
                        }
                        break;

                    case "slot_numbers_for_vehicles_with_color":
                        if (parkingLot != null && args.Length == 2)
                        {
                            string color = args[1];
                            var occupiedSlots = parkingLot.GetOccupiedSlots();
                            var slotNumbers = occupiedSlots
                                .Where(slot => slot.Color.Equals(color, StringComparison.OrdinalIgnoreCase))
                                .Select(slot => occupiedSlots.IndexOf(slot) + 1)
                                .ToList();
                            Console.WriteLine(string.Join(", ", slotNumbers));
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Usage: slot_numbers_for_vehicles_with_color [color]");
                        }
                        break;

                    case "slot_number_for_registration_number":
                        if (parkingLot != null && args.Length == 2)
                        {
                            string registrationNumber = args[1];
                            int slotNumber = parkingLot.GetSlotNumberByRegistrationNumber(registrationNumber);
                            if (slotNumber > 0)
                            {
                                Console.WriteLine(slotNumber);
                            }
                            else
                            {
                                Console.WriteLine("Not found");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Usage: slot_number_for_registration_number [registration_number]");
                        }
                        break;

                    case "exit":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }
            }
        }
    }
}
