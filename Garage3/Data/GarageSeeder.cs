using Garage3.Models;

namespace Garage3.Data
{
    public static class GarageSeeder
    {
        private static List<int> _vehicleTypeIds;

        private static readonly Random _random = new();
        public static void Seed(ApplicationDbContext context, IEnumerable<ApplicationUser> users)
        {
            SeedVehicleTypes(context);
            SeedVehicles(context, users);
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

        private static void SeedVehicles(ApplicationDbContext context, IEnumerable<ApplicationUser> users)
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
                    OwnerId = GetRandom(users.ToList())?.Id ?? string.Empty,
                    RegistrationNumber = GenerateUniqueRegNr(i),
                    Color = GetRandom(new List<string>() { "Blue", "Red", "Green", "Black", "White", "Yellow", "Pink" })!,
                    Brand = GetRandom(new List<string>() { "Volvo", "Saab", "BMW", "Volkswagen", "Toyota", "Mazda", "Audi", "Ford", "Yamaha", "Honda" })!,
                    Model = GetRandom(new List<string>() { "V70", "XC60", "9-3", "900", "Compact", "Raket", "320", "X5", "Golf", "Passat" })!,
                    WheelCount = GetRandom(new List<int>() { 2, 4, 6, 8, 12})
                });
            }
            context.Vehicles.AddRange(vehicles);
            context.SaveChanges();
        }

        private static int GenerateUniqueVehicleType(ApplicationDbContext context)
        {
            var vehicleTypeIds = _vehicleTypeIds == null ?
                context.VehicleTypes.Select(vt => vt.Id).ToList() : _vehicleTypeIds;

            if (!vehicleTypeIds.Any())
                throw new InvalidOperationException("No vehicle types available.");

            _vehicleTypeIds = vehicleTypeIds;
            
            return GetRandom<int>(vehicleTypeIds);

        }

        private static string GenerateUniqueRegNr(int index)
        {
            return $"DEV{index:000}";
        }

        private static T? GetRandom<T>(IList<T> list)
        {
            if(list == null || list.Count == 0)
            {
                return default;
            }
            return list[_random.Next(list.Count)];
        }
    }
}
