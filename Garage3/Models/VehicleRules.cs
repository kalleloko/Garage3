namespace Garage3.Models
{
    public static class VehicleRules
    {
        public const int RegistrationNumberMaxLength = 50;
        public const int ColorMaxLength = 50;
        public const int BrandMaxLength = 50;
        public const int ModelMaxLength = 50;
        public const int MinWheels = 2;
        public const int MaxWheels = 20;

        public static void FormatVehicle(Vehicle vehicle)
        {
            string color = vehicle.Color.Trim();
            vehicle.RegistrationNumber = vehicle.RegistrationNumber.Trim().ToUpper();
            vehicle.Color = color[..1].ToUpper() + color[1..].ToLower();
            vehicle.Brand = vehicle.Brand.Trim();
            vehicle.Model = vehicle.Model.Trim();
        }
    }
}
