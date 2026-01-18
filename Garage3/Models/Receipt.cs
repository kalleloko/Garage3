namespace Garage3.Models
{
	public class Receipt
	{
        public int VehicleId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string ParkingSpotNumber { get; set; } = string.Empty;
        public double Price { get; set; }
		public DateTime ArrivalTime { get; set; }
		public DateTime CheckoutTime { get; set; }
	}
}
