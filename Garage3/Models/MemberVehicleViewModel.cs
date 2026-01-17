namespace Garage3.Models
{
    public class MemberVehicleViewModel
    {
        public string RegistrationNumber { get; set; } = "";
        public string VehicleType { get; set; } = "";
        public string Brand { get; set; } = "";
        public string ParkingStatus { get; set; } = "";
        public DateTime? ArrivalTime { get; set; }
        public double AccumulatedCost { get; set; }
    }
}