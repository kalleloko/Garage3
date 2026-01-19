using Garage3.Models;
using Humanizer;
using System.Data.SqlTypes;

namespace Garage3.Helpers
{
	public static class PriceCalculation
	{
		// Extensionmetod för att beräkna priset för en parked bil
		public static double CalculatePrice(this Parking parking, double hourlyPrice, DateTime? checkoutTime = null)
		{
            var endTime = checkoutTime ?? parking.DepartTime;

            if (endTime == null)
                return 0;

            var timeParked = endTime.Value - parking.ArrivalTime;
            var totalCost = timeParked.TotalHours * hourlyPrice;

            return Math.Round(totalCost, 2);
        }
	}
}
