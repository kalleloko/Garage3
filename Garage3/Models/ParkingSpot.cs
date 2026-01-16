using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Garage3.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }

        [Display(Name = "Spot Number")]
        public string SpotNumber { get; set; } = default!;

        public ParkingSize SpotSize { get; set; } = default!;

        // Admin-controlled state (snow, maintenance, ...)
        public bool IsBlocked { get; set; }

        [Display(Name = "Occupied")]
        public bool IsOccupied => Parkings.Any(p => p.DepartTime == null);

        // Final availability
        public bool IsAvailable => !IsBlocked && !IsOccupied;

        // history of parkering
        public ICollection<Parking> Parkings { get; set; } = new List<Parking>();
    }
}