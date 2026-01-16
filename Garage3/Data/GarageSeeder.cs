using Garage3.Models;

namespace Garage3.Data
{
    public static class GarageSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            //TODO: implement
            //if (context.Vehicles.Any())
            //{
            //    return;
            //}

            //var vehicels = new List<Vehicle>();
            //var random = new Random();

            //for (int i = 0; i < 50; i++)
            //{
            //    vehicels.Add(new Vehicle
            //    {
            //        Type = VehicleType.Car,
            //        RegistrationNumber = GenerateUniqueRegNr(i),
            //        Color = "Blue",
            //        Brand = "Volvo",
            //        Model = "XC60",
            //        WheelCount = 4
            //    });
            //}
            //context.Vehicles.AddRange(vehicels);
            //context.SaveChanges();
        }
        private static string GenerateUniqueRegNr(int index)
        {
            return $"DEV{index:000}";
        }
    }
}
