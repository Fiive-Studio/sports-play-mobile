using FutbolPlay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using FutbolPlay.Views;
using Xamarin.Auth;
using FutbolPlay.WebApi.Services;
using FutbolPlay.WebApi.Model;
using T = FutbolPlay.Services.TranslateExtension;
using System.Threading.Tasks;

namespace FutbolPlay
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();

                if (Device.OS != TargetPlatform.Windows) { DependencyService.Get<ILocalize>().SetLocale(); }

                Page page = null;
                if (IsLogin) { page = new HomeView(); }
                else { page = new LoadingView(); }

                // The root page of your application
                MainPage = new NavigationPage(page) { BarBackgroundColor = GetBackgroundColor() };
                if (!IsLogin) { ValidateLoginAsync(); }
            }
            catch
            {
                MainPage.DisplayAlert(T.GetValue("app_name"), T.GetValue("error_message"), T.GetValue("ok"));
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #region Helpers

        private static Color GetBackgroundColor()
        {
            return Device.OnPlatform<Color>(Color.White, Color.Transparent, Color.Transparent);
        }

        // Navigation
        public async static void NavigateToHomeAsync(bool withoutModal = false)
        {
            if (!withoutModal) { await Current.MainPage.Navigation.PopModalAsync(); }
            if (IsLogin)
            {
                Current.MainPage.Navigation.InsertPageBefore(new HomeView(), Current.MainPage.Navigation.NavigationStack[0]);
                await Current.MainPage.Navigation.PopToRootAsync();
            }
        }
        public static async void RegisterSocialNetworkUserAsync(UserModel user)
        {
            try
            {
                bool response = await RestService.Instance.RegisterAsync(user);

                if (response) { IsLogin = true; }
                NavigateToHomeAsync();
            }
            catch { NavigateToHomeAsync(); }
        }
        public static void ShowStartView()
        {
            Current.MainPage.Navigation.InsertPageBefore(new StartView(), Current.MainPage.Navigation.NavigationStack[0]);
            Current.MainPage.Navigation.PopToRootAsync();
        }

        // Login
        public static bool IsLogin { get; set; }
        public static async void ValidateLoginAsync()
        {
            try
            {
                bool result = await SessionService.ValidateLoginAsync();
                if (result)
                {
                    IsLogin = true;
                    NavigateToHomeAsync(true);
                }
                else { ShowStartView(); }
            }
            catch { ShowStartView(); }
        }
        public static void LogOff()
        {
            SessionService.DeleteAccount();
            Current.MainPage = new NavigationPage(new StartView()) { BarBackgroundColor = GetBackgroundColor() };
            IsLogin = false;
        }

        #endregion
    }
}