using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FutbolPlay.WebApi.Services
{
    public class FunctionsService
    {
        public static bool ValidateEmail(string email)
        {
            const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

            return Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        public static bool ValidatePhone(string phone)
        {
            const string phoneRegex = @"^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$";

            return Regex.IsMatch(phone, phoneRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        public static DateTime GetDateTime(string date, string hour)
        {
            // Input Format date: mm/dd/yyyy HH:mm:ss
            // Input Format hour: HH:mm:ss

            string[] partsDate = date.Split(' ')[0].Split('/');
            string[] partsHour = hour.Split(':');

            DateTime dt = new DateTime
                (Convert.ToInt32(partsDate[2]), Convert.ToInt32(partsDate[0]), Convert.ToInt32(partsDate[1])
                , Convert.ToInt32(partsHour[0]), Convert.ToInt32(partsHour[1]), 0);
            return dt;
        }

        public static DateTime GetHour(string hour)
        {
            // Input Format: HH:mm

            string[] parts = hour.Split(':');
            DateTime dt = new DateTime(1986, 8, 20, Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), 0);
            return dt;
        }

        public static ReservationStatus GetStatus(int statusCode)
        {
            switch (statusCode)
            {
                case 1:
                    return ReservationStatus.Pending;
                case 2:
                    return ReservationStatus.Ok;
                case 3:
                    return ReservationStatus.CancelUser;
                case 4:
                    return ReservationStatus.CancelPlace;
                case 5:
                    return ReservationStatus.Running;
                case 6:
                    return ReservationStatus.Close;
            }

            return ReservationStatus.Error;
        }

        public static PitchType GetPitchType(int pitchType)
        {
            if (pitchType == 1) { return PitchType.Single; }
            else { return PitchType.Multiple; }
        }
    }
}
