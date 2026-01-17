namespace Garage3.Models
{
    internal class AdminParkedVehicleViewModel
    {
        public string OwnerName { get; set; }
        public string VehicleType { get; set; }
        public string RegistrationNumber { get; set; }
        public string ParkingSpotNumber { get; set; }

        public DateTime ArrivalTime { get; set; }
        public TimeSpan ParkedSince { get; set; }
    }
}