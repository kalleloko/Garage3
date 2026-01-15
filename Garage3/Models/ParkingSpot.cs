namespace Garage3.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public ICollection<Parking> Parkings { get; set; } = new List<Parking>();
    }
}