namespace Garage3.Models
{
    public class CheckoutConfirmViewModel
    {
        public int VehicleId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string ParkingSpotNumber { get; set; } = string.Empty;
        public DateTime ArrivalTime { get; set; }
    }
}
