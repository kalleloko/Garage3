using Garage3.Models;
using Humanizer;
using System.Data.SqlTypes;

namespace Garage3.Helpers
{
	public static class PriceCalculation
	{
		// Extensionmetod för att beräkna priset för en parked bil
		public static double CalculatePrice(this Parking parking, double hourlyPrice)
		{
            if (parking.DepartTime == null)
                return 0;

            var timeParked = parking.DepartTime.Value - parking.ArrivalTime;
            var totalCost = timeParked.TotalHours * hourlyPrice;

            return Math.Round(totalCost, 2);
        }
	}
}
