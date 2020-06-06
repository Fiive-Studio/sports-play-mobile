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
    public partial class PlaceView : ContentPage
    {
        public PlaceView(PlaceModel place)
        {
            InitializeComponent();
            BindingContext = new PlaceViewModel(this, place);
            AddPin(place);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => {
                await slMaps.FadeTo(0, 250, Easing.Linear);
                slMaps.IsVisible = false;
                slDescription.IsVisible = true;
                await slDescription.FadeTo(1, 250, Easing.Linear);
            };
            lblDescription.GestureRecognizers.Add(tapGestureRecognizer);

            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => {
                await slDescription.FadeTo(0, 250, Easing.Linear);
                slDescription.IsVisible = false;
                slMaps.IsVisible = true;
                await slMaps.FadeTo(1, 250, Easing.Linear);
            };
            lblMap.GestureRecognizers.Add(tapGestureRecognizer);
        }

        void AddPin(PlaceModel place)
        {
            var position = new Position(Convert.ToDouble(place.Latitude), Convert.ToDouble(place.Longitude));
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(1)));

            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = place.Name,
                Address = place.Address
            };

            map.Pins.Add(pin);
        }
    }
}
