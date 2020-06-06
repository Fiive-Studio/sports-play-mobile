using FutbolPlay.Services;
using FutbolPlay.Views;
using FutbolPlay.WebApi.Model;
using FutbolPlay.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FutbolPlay.ViewModels
{
    public class FindListViewModel : BaseViewModel
    {
        #region Vars

        List<ReservationModel> _reservations;
        bool _retryButtonStatus, _imageVisible;
        DateTime _date;

        #endregion

        #region Properties

        // Data
        public List<ReservationModel> Reservations
        {
            get { return _reservations; }
            private set
            {
                _reservations = value;
                OnPropertyChanged("Reservations");
            }
        }

        // Status
        public bool RetryButtonVisible
        {
            get { return _retryButtonStatus; }
            private set
            {
                _retryButtonStatus = value;
                OnPropertyChanged("RetryButtonVisible");
            }
        }
        public bool ImageVisible
        {
            get { return _imageVisible; }
            private set
            {
                _imageVisible = value;
                OnPropertyChanged("ImageVisible");
            }
        }

        // Actions
        public ICommand RetryCommand { get; set; }

        #endregion

        #region Methods

        public FindListViewModel(Page page, DateTime date) : base(page) { _date = date; InitControls(); }

        public FindListViewModel(Page page, DateTime date, int idPlace) : base(page) { _date = date; InitControls(); }

        public void InitControls() { RetryCommand = new Command(async () => { await FindPitches(); }); }

        public async Task OpenReservationAsync(ReservationModel reservation)
        {
            if (string.IsNullOrWhiteSpace(SessionService.Account.UserPhone))
            {
                await DisplayAlertAsync(MessagesTexts.user_without_phone);
                await OpenPageAsync(new ProfileView());
            }
            else { await OpenPageAsync(new ReservationCreateView(reservation)); }
        }

        public async Task FindPitches()
        {
            IsBusy = true;
            RetryButtonVisible = false;
            await Task.Delay(1000);

            try
            {
                Reservations = await RestService.Instance.FindReservationAsync(_date);
                if (Reservations == null)
                {
                    await DisplayAlertAsync(Services.MessagesTexts.error_message);
                    RetryButtonVisible = true;
                }
                else if (Reservations.Count == 0) { ImageVisible = true; }
            }
            catch
            {
                await DisplayAlertAsync(Services.MessagesTexts.error_message);
                RetryButtonVisible = true;
            }

            IsBusy = false;
        }

        #endregion 
    }
}
