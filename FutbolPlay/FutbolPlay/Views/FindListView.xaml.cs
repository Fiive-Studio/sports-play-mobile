using FutbolPlay.Layout;
using FutbolPlay.ViewModels;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FutbolPlay.Views
{
    public partial class FindListView : ContentPage
    {
        FindListViewModel _binding;

        public FindListView(DateTime date)
        {
            InitializeComponent();
            _binding = new FindListViewModel(this, date);
            _binding.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Reservations")
                {
                    if (_binding.Reservations == null) { return; }

                    lsPlaces.HasUnevenRows = true;
                    lsPlaces.ItemTemplate = new DataTemplate(typeof(FindCell));
                    lsPlaces.ItemsSource = _binding.Reservations;
                    lsPlaces.SeparatorColor = Color.FromHex("#ddd");
                    lsPlaces.IsRefreshing = false;
                }
            };

            lsPlaces.ItemTapped += async (sender, args) =>
            {
                await _binding.OpenReservationAsync((ReservationModel)args.Item);
                lsPlaces.SelectedItem = null;
            };

            lsPlaces.BackgroundColor = Color.Transparent;

            BindingContext = _binding;
            FindPitches();
        }

        async void FindPitches()
        {
            await _binding.FindPitches();
        }
    }
}
