using Garage3.Data;
using Garage3.Helpers;
using Garage3.Models;
using Microsoft.EntityFrameworkCore;

namespace Garage3.Services
{
	public class ReceiptService
	{
		public double HourlyParkingPrice { get; private set; } = 15.00;

		public Receipt CreateReceipt(Vehicle vehicle, DateTime checkoutTime)
		{
			var price = vehicle.CalculatePrice(checkoutTime, HourlyParkingPrice);

			return new Receipt()
			{
				RegistrationNumber = vehicle.RegistrationNumber,
				Price = price,
				ArrivalTime = vehicle.ArrivalTime,
				CheckoutTime = checkoutTime

			};
		}
	}
}
