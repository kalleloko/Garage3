using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garage3.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        public int VehicleTypeId { get; set; }

        [ValidateNever]
        public VehicleType Type { get; set; }

        public string OwnerId { get; set; } = string.Empty;

        [ValidateNever]
        public ApplicationUser Owner { get; set; } = null!;


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

        [NotMapped]
        public Parking? ActiveParking => Parkings.SingleOrDefault(p => p.DepartTime == null);
    }
}
