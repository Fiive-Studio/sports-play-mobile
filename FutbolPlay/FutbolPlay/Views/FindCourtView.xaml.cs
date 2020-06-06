using FutbolPlay.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FutbolPlay.Views
{
    public partial class FindCourtView : ContentPage
    {
        public FindCourtView()
        {
            InitializeComponent();
            BindingContext = new FindCourtViewModel(this);

            dpDate.MinimumDate = DateTime.Now;
        }
    }
}
