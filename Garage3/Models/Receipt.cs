namespace Garage3.Models
{
	public class Receipt
	{
		public string RegistrationNumber { get; set; } = string.Empty;
		public double Price { get; set; }
		public DateTime ArrivalTime { get; set; }
		public DateTime CheckoutTime { get; set; }
	}
}
