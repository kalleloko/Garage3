using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Garage3.Models
{
    [Index(nameof(RegistrationNumber), IsUnique = true)]
    public class Vehicle
    {
        public int Id { get; set; }

        public int VehicleTypeId { get; set; }
        public VehicleType Type { get; set; }

        [Required]
        [StringLength(VehicleRules.RegistrationNumberMaxLength)]
        [Display(Name = "Registration Number")]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "The registration number must only contain A-Z, 0-9 and space.")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [StringLength(VehicleRules.ColorMaxLength)]
        public string Color { get; set; } = string.Empty;

        [StringLength(VehicleRules.BrandMaxLength)]
        public string Brand { get; set; } = string.Empty;

        [StringLength(VehicleRules.ModelMaxLength)]
        public string Model { get; set; } = string.Empty;

        [Range(VehicleRules.MinWheels, VehicleRules.MaxWheels)]
        [Display(Name = "Wheel Count")]
        public int WheelCount { get; set; }

        [Display(Name = "Arrival Time")]
        public DateTime? ArrivalTime => ActiveParking?.ArrivalTime ?? null;

        public ICollection<Parking> Parkings { get; set; } = new List<Parking>();

        public Parking? ActiveParking => Parkings.SingleOrDefault(p => p.DepartTime == null);
    }
}
