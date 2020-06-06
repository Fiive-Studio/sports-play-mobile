using FutbolPlay.Layout;
using FutbolPlay.ViewModels;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using I = FutbolPlay.Services.ImageResourceExtension;

namespace FutbolPlay.Views
{
    public partial class PlacesListView : ContentPage
    {
        PlacesListViewModel _binding;

        public PlacesListView()
        {
            InitializeComponent();
            _binding = new PlacesListViewModel(this);
            _binding.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Places")
                {
                    if (_binding.Places == null) { return; }

                    lsPlaces.HasUnevenRows = true;
                    lsPlaces.ItemTemplate = new DataTemplate(typeof(PlaceCell));
                    lsPlaces.ItemsSource = _binding.Places;
                    lsPlaces.SeparatorColor = Color.FromHex("#ddd");
                    lsPlaces.IsRefreshing = false;
                }
            };

            lsPlaces.ItemTapped += async (sender, args) =>
            {
                await _binding.OpenPlaceAsync((PlaceModel)args.Item);
                lsPlaces.SelectedItem = null;
            };

            lsPlaces.BackgroundColor = Color.Transparent;

            BindingContext = _binding;
            GetPlaces();
        }

        async void GetPlaces()
        {
            await _binding.GetPlaces();
        }
    }
}
