using FutbolPlay.Views;
using FutbolPlay.WebApi.Services;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlay.Services.TranslateExtension;
using FutbolPlay.Services;

namespace FutbolPlay.ViewModels
{
    public class PlaceViewModel : BaseViewModel
    {
        #region Vars

        PlaceModel _place;
        HourModel _hourOfDay;

        #endregion

        #region Properties

        // Data
        public PlaceModel Place
        {
            get { return _place; }
            private set
            {
                _place = value;
                OnPropertyChanged("Place");
                OnPropertyChanged("Time");
            }
        }

        public string Time {
            get
            {
                if (Place != null)
                {
                    return string.Format("{0}: {1:hh:mm tt} - {2:hh:mm tt}", T.GetValue("opening_today"), _hourOfDay.HourStart, _hourOfDay.HourEnd);
                }

                return string.Empty;
            }
        }

        // Actions
        public ICommand OpenMapCommand { get; set; }
        public ICommand ShowTimesCommand { get; set; }
        public ICommand FindCommand { get; set; }

        #endregion

        #region Methods

        public PlaceViewModel(Page page, PlaceModel place) : base(page)
        {
            Place = place;
            _hourOfDay = place.GetHourOfDay();

            FindCommand = new Command(async () => { await OpenPageAsync(new FindCourtView()); });
            OpenMapCommand = new Command(async () => { await OpenPageAsync(new MapView(_place)); });
            ShowTimesCommand = new Command(async () =>
            {
                string[] times = new string[place.Hours.Count];
                int count = 0;
                foreach (HourModel hour in place.Hours)
                {
                    int numberDay = hour.NumberDay;
                    if (numberDay == 7) { numberDay = 0; }

                    string day = null;
                    if (numberDay == 8) { day = T.GetValue("holydays"); }
                    else { day = place.GetDayName(numberDay, DependencyService.Get<ILocalize>().GetCurrentCultureInfo()); }

                    times[count] = string.Format("{0}: {1:hh:mm tt} - {2:hh:mm tt}", day, hour.HourStart, hour.HourEnd);
                    count++;
                }

                await _page.DisplayActionSheet("", T.GetValue("close"), null, times);
            });
        }

        #endregion
    }
}
