using FutbolPlay.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using I = FutbolPlay.Services.ImageResourceExtension;

namespace FutbolPlay.Layout
{
    public class PlaceCell : ViewCell
    {
        public PlaceCell()
        {
            var logoPlace = new Image
            {
                HeightRequest = 50,
                WidthRequest = 50,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            logoPlace.SetBinding(Image.SourceProperty, "Image");

            var namePlace = new Label()
            {
                FontFamily = "HelveticaNeue-Medium",
                FontSize = Device.OnPlatform<double>(20, 22, 22),
                TextColor = Color.Black
            };
            namePlace.SetBinding(Label.TextProperty, "Name");

            var addressPlace = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = 12,
                TextColor = Color.FromHex("#666")
            };
            addressPlace.SetBinding(Label.TextProperty, new Binding("Address"));

            var detailPlaceLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { addressPlace }
            };

            var infoPlaceLayout = new StackLayout
            {
                Padding = new Thickness(10, 0, 0, 0),
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { namePlace, detailPlaceLayout }
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
                Children = { logoPlace, infoPlaceLayout, tapImage },
                BackgroundColor = Color.Transparent
            };

            this.View = cellLayout;
        }
    }
}
