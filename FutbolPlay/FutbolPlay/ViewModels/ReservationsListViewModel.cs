using FutbolPlay.Services;
using FutbolPlay.Views;
using FutbolPlay.WebApi.Model;
using FutbolPlay.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlay.Services.TranslateExtension;

namespace FutbolPlay.ViewModels
{
    public class ReservationsListViewModel : BaseViewModel
    {
        #region Vars

        ObservableCollection<ReservationModel> _reservations;
        bool _retryButtonStatus, _imageVisible;

        #endregion

        #region Properties

        // Data
        public ObservableCollection<ReservationModel> Reservations
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
        public ICommand RefreshCommand { get; set; }

        #endregion

        #region Methods

        public ReservationsListViewModel(Page page) : base(page)
        {
            RetryCommand = new Command(async () => { await GetReservations(); });
            RefreshCommand = new Command(async () => { await GetReservations(false); });
        }

        public async Task OpenReservationSummaryAsync(ReservationModel reservation) { await OpenPageAsync(new ReservationSummaryView(reservation)); }

        public async Task GetReservations(bool showLoad = true)
        {
            IsBusy = showLoad;
            RetryButtonVisible = false;
            await Task.Delay(1000);

            try
            {
                Reservations = new ObservableCollection<ReservationModel>(await RestService.Instance.GetMyReservationsAsync());
                if (Reservations == null)
                {
                    await DisplayAlertAsync(MessagesTexts.error_message);
                    RetryButtonVisible = true;
                }
                else if (Reservations.Count == 0) { ImageVisible = true; }
            }
            catch
            {
                await DisplayAlertAsync(MessagesTexts.error_message);
                RetryButtonVisible = true;
            }

            IsBusy = false;
        }

        #endregion 
    }
}
