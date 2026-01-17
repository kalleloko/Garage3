namespace Garage3.Models
{
    internal class MemberListViewModel
    {
        public string MemberId { get; set; } = "";
        public string MemberName { get; set; } = "";
        public int VehicleCount { get; set; }
        public double TotalParkingCost { get; set; }
    }
}