using Garage3.Models;

namespace Garage3.Services
{
    public class ParkingService
    {
        public Parking StartParking(Vehicle vehicle, ParkingSpot spot)
        {
            if (vehicle.Parkings.Any(p => p.DepartTime == null))
                throw new InvalidOperationException("Vehicle already parked.");

            return new Parking
            {
                Vehicle = vehicle,
                ParkingSpot = spot,
                ArrivalTime = DateTime.Now
            };
        }
    }
}
