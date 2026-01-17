using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace Garage3.Models
{
    public class UserParkVehicleViewModel
    {
        public int VehicleId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        public int ParkingSpotId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> FreeParkingSpots { get; set; }
    }
}
