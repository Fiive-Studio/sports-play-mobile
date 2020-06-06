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
    public partial class ReservationCreateView : ContentPage
    {
        public ReservationCreateView(ReservationModel reservation)
        {
            InitializeComponent();
            BindingContext = new ReservationCreateViewModel(this, reservation);
        }
    }
}
