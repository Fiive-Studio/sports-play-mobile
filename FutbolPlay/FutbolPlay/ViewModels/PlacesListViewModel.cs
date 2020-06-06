using FutbolPlay.Views;
using FutbolPlay.WebApi.Services;
using FutbolPlay.WebApi.Model;
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
    public class PlacesListViewModel : BaseViewModel
    {
        #region Vars

        List<PlaceModel> _places;
        bool _retryButtonStatus;

        #endregion

        #region Properties

        // Data
        public List<PlaceModel> Places
        {
            get { return _places; }
            private set
            {
                _places = value;
                OnPropertyChanged("Places");
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

        // Actions
        public ICommand RetryCommand { get; set; }

        #endregion

        #region Methods

        public PlacesListViewModel(Page page) : base(page)
        {
            RetryCommand = new Command(async () => { await GetPlaces(); });
        }

        public async Task OpenPlaceAsync(PlaceModel place) { await OpenPageAsync(new PlaceView(place)); }

        public async Task GetPlaces()
        {
            if (SessionService.Places == null)
            {
                IsBusy = true;
                RetryButtonVisible = false;
                await Task.Delay(1000);
            }

            try
            {
                Places = await RestService.Instance.GetPlacesAsync();

                if (Places == null)
                {
                    await DisplayAlertAsync(Services.MessagesTexts.error_message);
                    RetryButtonVisible = true;
                }
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
