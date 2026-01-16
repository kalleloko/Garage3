namespace Garage3.Models
{
    public class Parking
    {
        public int Id { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = default!;

        public int ParkingSpotId { get; set; }
        public ParkingSpot ParkingSpot { get; set; } = default!;

        public DateTime ArrivalTime { get; set; }
        public DateTime? DepartTime { get; set; }
        public bool IsActive => DepartTime == null;

    }
}