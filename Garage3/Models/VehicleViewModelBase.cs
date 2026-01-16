using System.ComponentModel.DataAnnotations;

namespace Garage3.Models
{
    public abstract class VehicleViewModelBase
    {
        public int Id { get; set; }

        public VehicleType Type { get; set; }

        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Display(Name = "Arrival Time")]
        public DateTime ArrivalTime { get; set; }

        [Display(Name = "Parking Time")]
        [DataType(DataType.Time)]
        public TimeSpan ParkedTime => DateTime.Now > ArrivalTime ? DateTime.Now - ArrivalTime : TimeSpan.Zero;
        public string ParkedTimeDisplay
        {
            get
            {
                var t = ParkedTime;

                if (t.TotalSeconds < 60)
                {
                    // less than 1 minute
                    return $"{t.Seconds} sec";
                }
                else if (t.TotalMinutes < 60)
                {
                    // less than 1 hour
                    return $"{t.Minutes} min";
                }
                else if (t.TotalHours < 24)
                {
                    // less than 24 hours
                    return $"{(int)t.TotalHours:D2}:{t.Minutes:D2}";
                }
                else
                {
                    // 24 hours or more
                    int days = t.Days;
                    int hours = t.Hours;
                    int minutes = t.Minutes;
                    return $"{days} day{(days > 1 ? "s" : "")} {hours:D2}:{minutes:D2}";
                }
            }
        }
    }
}