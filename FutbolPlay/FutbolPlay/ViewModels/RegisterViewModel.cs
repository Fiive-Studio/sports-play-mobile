using FutbolPlay.WebApi.Services;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlay.Services.TranslateExtension;
using FutbolPlay.Services;

namespace FutbolPlay.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        #region Vars

        string _name, _mail, _phone, _password, _repassword;

        #endregion

        #region Properties

        // Entries
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("RegisterButtonStatus");
            }
        }
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged("RegisterButtonStatus");
            }
        }
        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                OnPropertyChanged("RegisterButtonStatus");
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("RegisterButtonStatus");
            }
        }
        public string RePassword
        {
            get { return _repassword; }
            set
            {
                _repassword = value;
                OnPropertyChanged("RegisterButtonStatus");
            }
        }

        // Status
        public bool RegisterButtonStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name) ||
                    string.IsNullOrWhiteSpace(Mail) ||
                    string.IsNullOrWhiteSpace(Phone) ||
                    string.IsNullOrEmpty(Password) ||
                    string.IsNullOrEmpty(RePassword)) { return false; }

                if (Password.Length < 6) { return false; }

                return !IsBusy;
            }
        }

        // Actions
        public ICommand RegisterCommand { get; set; }

        #endregion

        #region Methods

        public RegisterViewModel(Page page) : base(page)
        {
            RegisterCommand = new Command(async () => {

                bool result = await RegisterValidations();
                if (!result) { return; }

                IsBusy = true;
                var user = new UserModel { Name = Name, Mail = Mail, Phone = Phone, Password = Password };

                try
                {
                    bool response = true;
                    response = await RestService.Instance.RegisterAsync(user);

                    if (response)
                    {
                        App.IsLogin = true;
                        App.NavigateToHomeAsync(true);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(RestService.Instance.ErrorCode))
                        {
                            await DisplayAlertAsync(GetMessageCode(RestService.Instance.ErrorCode));
                        }
                        else { await DisplayAlertAsync(MessagesTexts.error_message); }
                    }
                }
                catch { await DisplayAlertAsync(MessagesTexts.error_message); }

                IsBusy = false;
            });
        }

        async Task<bool> RegisterValidations()
        {
            if (!FunctionsService.ValidateEmail(Mail))
            {
                await _page.DisplayAlert(T.GetValue("app_name"), T.GetValue("mail_error"), T.GetValue("ok"));
                return false;
            }

            if (!FunctionsService.ValidatePhone(Phone))
            {
                await _page.DisplayAlert(T.GetValue("app_name"), T.GetValue("phone_error"), T.GetValue("ok"));
                return false;
            }

            if (Password != RePassword)
            {
                await _page.DisplayAlert(T.GetValue("app_name"), T.GetValue("password_differents_message"), T.GetValue("ok"));
                return false;
            }

            return true;
        }

        protected override void UpdateAdditionalProperties()
        {
            OnPropertyChanged("RegisterButtonStatus");
        }

        #endregion
    }
}
