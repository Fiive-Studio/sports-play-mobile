using FutbolPlay.Services;
using FutbolPlay.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FutbolPlay.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Properties

        // Actions
        public ICommand FindCommand { get; set; }
        public ICommand CourtsCommand { get; set; }
        public ICommand MapCommand { get; set; }
        public ICommand MyReservationCommand { get; set; }
        public ICommand ProfileCommand { get; set; }
        public ICommand LogoffCommand { get; set; }

        #endregion

        #region Methods

        public HomeViewModel(Page page) : base(page)
        {
            FindCommand = new Command(async () => { await OpenPageAsync(new FindCourtView()); });
            CourtsCommand = new Command(async() => { await OpenPageAsync(new PlacesListView()); });
            MapCommand = new Command(async () => { await OpenPageAsync(new MapView()); });
            MyReservationCommand = new Command(async () => { ReservationService.HasChanges = false; await OpenPageAsync(new ReservationsListView()); });
            ProfileCommand = new Command(async () => { await OpenPageAsync(new ProfileView()); });
            LogoffCommand = new Command(() => { App.LogOff(); });
        }

        #endregion
    }
}
