using FutbolPlay.Views;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FutbolPlay.ViewModels
{
    public class FindCourtViewModel : BaseViewModel
    {
        #region Vars

        DateTime _date;
        TimeSpan _hour;
        bool _imageVisible;

        #endregion


        #region Properties

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        public TimeSpan Hour
        {
            get { return _hour; }
            set
            {
                _hour = value;
                OnPropertyChanged("Time");
            }
        }


        // Command
        public ICommand FindCommand { get; set; }

        #endregion

        public FindCourtViewModel(Page page) : base(page)
        {
            int hour = DateTime.Now.Hour;
            Date = DateTime.Now;

            if (hour == 23)
            {
                Hour = new TimeSpan(0, 0, 0);
                Date = Date.AddDays(1);
            }
            else { Hour = new TimeSpan(DateTime.Now.Hour + 1, 0, 0); }

            FindCommand = new Command(async () =>
            {
                DateTime timeToSearch = new DateTime(Date.Year, Date.Month, Date.Day, Hour.Hours, 0, 0);
                if (timeToSearch < DateTime.Now) { await DisplayAlertAsync(Services.MessagesTexts.datetime_is_past); }
                else { await OpenPageAsync(new FindListView(timeToSearch)); }
            });
        }
    }
}
