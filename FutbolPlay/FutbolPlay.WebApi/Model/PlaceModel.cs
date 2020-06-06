using FutbolPlay.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FutbolPlay.WebApi.Model
{
    public class PlaceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public int MaxDaysReservation { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public int FormatHour { get; set; }
        public List<HourModel> Hours { get; set; }
        public bool HasMutiple { get; set; }
        public string ProfileImgUrl { get; set; }
        public ImageSource Image { get { return ImageSource.FromUri(new Uri(string.Concat(RestService.Instance.DefaultImgUri, ProfileImgUrl))); } }

        public HourModel GetHourOfDay() { return GetHourOfDay(DateTime.Now.DayOfWeek); }

        public HourModel GetHourOfDay(DayOfWeek dayOfWeek)
        {
            int day = (int)dayOfWeek;
            if (day == 0) { day = 7; }
            var hour = (from d in Hours where d.NumberDay == day select d).SingleOrDefault();
            return hour;
        }

        public string GetDayName(int dayOfWeek, CultureInfo culture)
        {
            DayOfWeek day = (DayOfWeek)dayOfWeek;
            return culture.DateTimeFormat.GetDayName(day);
        }
    }

    public class HourModel
    {
        public int NumberDay { get; set; }
        public DateTime HourStart { get; set; }
        public DateTime HourEnd { get; set; }
    }
}
