using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using I = FutbolPlay.Services.ImageResourceExtension;

namespace FutbolPlay.Layout
{
    class FindCell : ViewCell
    {
        public FindCell()
        {
            var logoPlace = new Image
            {
                HeightRequest = 50,
                WidthRequest = 50,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            logoPlace.SetBinding(Image.SourceProperty, "Place.Image");

            var namePlace = new Label()
            {
                FontFamily = "HelveticaNeue-Medium",
                FontSize = 18,
                TextColor = Color.Black
            };
            namePlace.SetBinding(Label.TextProperty, "Place.Name");

            var dateReservation = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = 12,
                TextColor = Color.FromHex("#666")
            };
            dateReservation.SetBinding(Label.TextProperty, new Binding("Date", stringFormat: "{0:yyyy-MM-dd hh:mm tt}"));

            var pitchDescription = new Label()
            {
                FontSize = 12,
                TextColor = Color.Gray
            };
            pitchDescription.SetBinding(Label.TextProperty, "Pitch.Description");

            var priceReservation = new Label()
            {
                FontSize = 12,
                TextColor = Color.Gray
            };
            priceReservation.SetBinding(Label.TextProperty, "Price");

            var statusLayout = new StackLayout()
            {
                Spacing = 3,
                Orientation = StackOrientation.Horizontal,
                Children = { pitchDescription, priceReservation }
            };

            var detailReservationLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { dateReservation }
            };

            var infoReservation = new StackLayout
            {
                Padding = new Thickness(10, 0, 0, 0),
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { namePlace, detailReservationLayout, statusLayout }
            };

            var tapImage = new Image()
            {
                Source = ImageSource.FromResource(string.Concat(I.ImagePath, "next.png")),
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = 15
            };

            var cellLayout = new StackLayout
            {
                Spacing = 0,
                Padding = new Thickness(10, 5, 10, 5),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { logoPlace, infoReservation, tapImage }
            };

            this.View = cellLayout;
        }
    }
}
