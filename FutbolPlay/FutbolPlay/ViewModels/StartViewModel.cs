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
    public class StartViewModel : BaseViewModel
    {
        public ICommand RegisterMailCommand { get; set; }
        public ICommand FacebookRegisterCommand { get; set; }
        public ICommand PlacesCommand { get; set; }
        public ICommand LoginCommand { get; set; }

        public StartViewModel(Page page) : base(page)
        {
            RegisterMailCommand = new Command(async () => { await OpenPageAsync(new RegisterView()); });
            FacebookRegisterCommand = new Command(async () => { await _navigation.PushModalAsync(new FacebookLoginView()); });
            PlacesCommand = new Command(async () => { await OpenPageAsync(new PlacesListView()); });
            LoginCommand = new Command(async () => { await OpenPageAsync(new LoginView()); });
        }
    }
}
