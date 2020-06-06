using FutbolPlay.ViewModels;
using FutbolPlay.WebApi.Model;
using FutbolPlay.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace FutbolPlay.Views
{
    public partial class MapView : ContentPage
    {
        BaseViewModel _baseModel;

        public MapView()
        {
            InitializeComponent();

            _baseModel = new BaseViewModel(this);
            var position = new Position(3.4209706, -76.5152019);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(3)));
            LoadAllPlaces();
        }

        public MapView(PlaceModel place)
        {
            InitializeComponent();
            Title = place.Name;

            var position = new Position(Convert.ToDouble(place.Latitude), Convert.ToDouble(place.Longitude));
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(1)));
            AddPin(place, position, false);
        }

        async void LoadAllPlaces()
        {
            try
            {
                List<PlaceModel> places = await RestService.Instance.GetPlacesAsync();
                if (places == null) { await _baseModel.DisplayAlertAsync(Services.MessagesTexts.error_message); }
                else
                {
                    foreach (PlaceModel place in places)
                    {
                        var position = new Position(Convert.ToDouble(place.Latitude), Convert.ToDouble(place.Longitude));
                        AddPin(place, position, true);
                    }
                }
            }
            catch
            {
                await _baseModel.DisplayAlertAsync(Services.MessagesTexts.error_message);
            }
        }

        void AddPin(PlaceModel place, Position position, bool addEvent)
        {
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = place.Name,
                Address = place.Address
            };

            if (addEvent) { pin.Clicked += async (sender, e) => { await Navigation.PushAsync(new PlaceView(place)); }; }

            map.Pins.Add(pin);
        }
    }
}
