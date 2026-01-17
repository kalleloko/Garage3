namespace Garage3.Models
{
    internal class MemberDetailsViewModel
    {
        public string MemberName { get; set; } = "";
        public List<MemberVehicleViewModel> Vehicles { get; set; } = new();
    }
}