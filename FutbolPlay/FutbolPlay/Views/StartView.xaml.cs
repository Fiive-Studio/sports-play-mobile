using FutbolPlay.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FutbolPlay.Views
{
    public partial class StartView : ContentPage
    {
        public StartView()
        {
            InitializeComponent();
            BindingContext = new StartViewModel(this); 
        }
    }
}
