using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using T = FutbolPlay.Services.TranslateExtension;

namespace FutbolPlay.Converters
{
    public class ReservationStatusValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReservationStatus rs = (ReservationStatus)value;
            switch (rs)
            {
                case ReservationStatus.Pending: return T.GetValue("reservation_pending_short");
                case ReservationStatus.Ok: return T.GetValue("reservation_ok_short");
                case ReservationStatus.CancelPlace: return T.GetValue("reservation_cancel_place_short");
                case ReservationStatus.CancelUser: return T.GetValue("reservation_cancel_user_short");
                case ReservationStatus.Running: return T.GetValue("reservation_running_short");
                case ReservationStatus.Close: return T.GetValue("reservation_close_short");
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
