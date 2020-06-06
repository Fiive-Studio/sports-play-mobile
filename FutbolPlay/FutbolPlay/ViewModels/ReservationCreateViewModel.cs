using FutbolPlay.Services;
using FutbolPlay.WebApi.Model;
using FutbolPlay.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlay.Services.TranslateExtension;

namespace FutbolPlay.ViewModels
{
    public class ReservationCreateViewModel : BaseViewModel
    {
        #region Vars

        ReservationModel _reservation;
        bool _reservationButtonStatus;

        #endregion

        #region Properties

        // Data
        public string UserName { get; private set; }

        public ReservationModel Reservation
        {
            get { return _reservation; }
        }

        public string Date
        {
            get
            {
                return _reservation.Date.ToString("ddd, dd MMMM hh:mm tt");
            }
        }

        public string Value
        {
            get
            {
                return Reservation.Price.ToString("C", new CultureInfo("es-CO"));
            }
        }

        // Status
        public bool ReservationButtonStatus
        {
            get { return _reservationButtonStatus; }
            set
            {
                _reservationButtonStatus = value;
                OnPropertyChanged("ReservationButtonStatus");
            }
        }

        // Actions
        public ICommand DoReservationCommand { get; set; }

        #endregion

        #region Methods

        public ReservationCreateViewModel(Page page, ReservationModel reservation) : base(page)
        {
            _reservation = reservation;
            ReservationButtonStatus = true;

            string[] nameParts = SessionService.Account.UserName.Split(' ');
            if (nameParts.Length > 1) { UserName = string.Concat(nameParts[0], " ", nameParts[1]); }
            else { UserName = nameParts[0]; }

            DoReservationCommand = new Command(async () => 
            {
                ReservationButtonStatus = false;
                IsBusy = true;

                try
                {
                    ReservationStatus response;
                    if (Reservation.Pitch.PitchType == PitchType.Single)
                    {
                        response = await RestService.Instance.DoReservationSingleAsync(reservation);
                    }
                    else { response = await RestService.Instance.DoReservationMultipleAsync(reservation); }

                    MessagesTexts message = MessagesTexts.error_message;
                    if (response == ReservationStatus.Pending){ message = MessagesTexts.reservation_pending; }
                    else if (response == ReservationStatus.Conflict) { message = MessagesTexts.reservation_conflict; }
                    else if (response == ReservationStatus.Ok) { message = MessagesTexts.reservation_ok; }

                    await DisplayAlertAsync(message);

                    if (response == ReservationStatus.Ok || response == ReservationStatus.Pending) { App.NavigateToHomeAsync(true); }
                }
                catch { await DisplayAlertAsync(MessagesTexts.error_message); }

                IsBusy = false;
            });
        }

        protected override void UpdateAdditionalProperties() { }

        #endregion
    }
}
