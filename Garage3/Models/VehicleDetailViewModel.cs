using System.ComponentModel.DataAnnotations;

namespace Garage3.Models
{
    public class VehicleDetailViewModel : VehicleViewModelBase
    {
        public string Color { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;

        [Display(Name = "Wheel Count")]
        public int WheelCount { get; set; }
    }
}
