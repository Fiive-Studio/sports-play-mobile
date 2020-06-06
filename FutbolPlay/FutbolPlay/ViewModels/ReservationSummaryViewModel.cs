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
    public class ReservationSummaryViewModel : BaseViewModel
    {
        #region Vars

        ReservationModel _reservation;
        bool _cancelReservationButtonStatus;

        #endregion

        #region Properties

        // Data
        public ReservationModel Reservation
        {
            get { return _reservation; }
            set
            {
                _reservation = value;
                OnPropertyChanged("Reservation");
                OnPropertyChanged("ReservationStatusId");
                OnPropertyChanged("Price");
            }
        }
        public string Price
        {
            get
            {
                if (Reservation == null) { return string.Empty; }
                return Reservation.Price.ToString("C", new CultureInfo("es-CO"));
            }
        }

        // Status
        public bool CancelReservationButtonStatus
        {
            get { return _cancelReservationButtonStatus; }
            set
            {
                _cancelReservationButtonStatus = value;
                OnPropertyChanged("CancelReservationButtonStatus");
            }
        }

        // Actions
        public ICommand CancelReservationCommand { get; set; }

        #endregion

        #region Methods

        public ReservationSummaryViewModel(Page page, ReservationModel reservation) : base(page)
        {
            _reservation = reservation;
            if (reservation.Status == ReservationStatus.Pending || reservation.Status == ReservationStatus.Ok) { CancelReservationButtonStatus = true; }
            else { CancelReservationButtonStatus = false; }

            CancelReservationCommand = new Command(async () =>
            {
                bool result = await _page.DisplayAlert(T.GetValue("app_name"), T.GetValue("reservation_cancel"), T.GetValue("ok"), T.GetValue("cancel"));
                if (result)
                {
                    CancelReservationButtonStatus = false;
                    IsBusy = true;

                    try
                    {
                        bool response = await RestService.Instance.CancelReservationAsync(reservation.IdReservation);

                        if (response)
                        {
                            ReservationService.HasChanges = true;
                            await DisplayAlertAsync(MessagesTexts.reservation_cancel_ok);
                        }
                        else
                        {
                            await DisplayAlertAsync(MessagesTexts.error_message);
                            CancelReservationButtonStatus = true;
                        }
                    }
                    catch
                    {
                        await DisplayAlertAsync(MessagesTexts.error_message);
                        CancelReservationButtonStatus = true;
                    }

                    IsBusy = false;
                }
            });
        }

        protected override void UpdateAdditionalProperties() { }

        #endregion
    }
}
