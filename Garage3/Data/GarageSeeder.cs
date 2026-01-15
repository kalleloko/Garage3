using Garage3.Models;

namespace Garage3.Data
{
    public static class GarageSeeder
    {
        private static List<int> _vehicleTypeIds;
        public static void Seed(ApplicationDbContext context)
        {
            SeedVehicleTypes(context);
            SeedVehicles(context);
        }

        private static void SeedVehicleTypes(ApplicationDbContext context)
        {
            VehicleType[] vehicleTypes =
            {
                new VehicleType { Name = "Car" },
                new VehicleType { Name = "Motorcycle" },
                new VehicleType { Name = "Truck" },
                new VehicleType { Name = "Bus" }
            };

            var existingTypes = context.VehicleTypes.Select(vt => vt.Name).ToHashSet();
            var newTypes = vehicleTypes.Where(vt => !existingTypes.Contains(vt.Name)).ToList();
            if (newTypes.Any())
            {
                context.VehicleTypes.AddRange(newTypes);
                context.SaveChanges();
            }
        }

        private static void SeedVehicles(ApplicationDbContext context)
        {
            if (context.Vehicles.Any())
            {
                return; // DB has been seeded
            }
            var vehicles = new List<Vehicle>();
            

            for (int i = 0; i < 50; i++)
            {
                vehicles.Add(new Vehicle
                {
                    VehicleTypeId = GenerateUniqueVehicleType(context),
                    RegistrationNumber = GenerateUniqueRegNr(i),
                    Color = "Blue",
                    Brand = "Volvo",
                    Model = "XC60",
                    WheelCount = 4
                });
            }
            context.Vehicles.AddRange(vehicles);
            context.SaveChanges();
        }

        private static int GenerateUniqueVehicleType(ApplicationDbContext context)
        {
            var random = new Random();
            var vehicleTypeIds = _vehicleTypeIds == null ?
                context.VehicleTypes.Select(vt => vt.Id).ToList() : _vehicleTypeIds;

            if (!vehicleTypeIds.Any())
                throw new InvalidOperationException("No vehicle types available.");

            _vehicleTypeIds = vehicleTypeIds;
            
            return vehicleTypeIds[random.Next(vehicleTypeIds.Count)];

        }

        private static string GenerateUniqueRegNr(int index)
        {
            return $"DEV{index:000}";
        }
    }
}
