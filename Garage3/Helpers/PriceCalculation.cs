using Garage3.Models;
using Humanizer;
using System.Data.SqlTypes;

namespace Garage3.Helpers
{
	public static class PriceCalculation
	{
		// Extensionmetod för att beräkna priset för en parked bil
		public static double CalculatePrice(this Vehicle vehicle, DateTime checkoutTime, double hourlyPrice)
		{
            var arrivalTime = vehicle.ArrivalTime ?? throw new ArgumentException("Vehicle has no arrival time.");
            var timeParked = checkoutTime.Subtract(arrivalTime);

			var totalCost = timeParked.TotalHours * hourlyPrice;
			totalCost = Math.Round(totalCost, 2);

			return totalCost;
		}
	}
}
