using FutbolPlay.Views;
using FutbolPlay.WebApi.Services;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlay.Services.TranslateExtension;
using FutbolPlay.Services;

namespace FutbolPlay.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        #region Vars

        UserModel _user;

        #endregion

        #region Properties

        // Model
        public UserModel User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("Mail");
                OnPropertyChanged("Phone");
                OnPropertyChanged("ChangePasswordVisible");
                OnPropertyChanged("UpdateButtonStatus");
            }
        }

        // Entries
        public string Name
        {
            get
            {
                if (User == null) { return string.Empty; }
                return _user.Name;
            }
            set
            {
                _user.Name = value;
                OnPropertyChanged("UpdateButtonStatus");
            }
        }
        public string Mail
        {
            get
            {
                if (User == null) { return string.Empty; }
                return _user.Mail;
            }
            set
            {
                _user.Mail = value;
                OnPropertyChanged("UpdateButtonStatus");
            }
        }
        public string Phone
        {
            get
            {
                if (User == null) { return string.Empty; }
                return _user.Phone;
            }
            set
            {
                _user.Phone = value;
                OnPropertyChanged("UpdateButtonStatus");
            }
        }

        // Status
        public bool UpdateButtonStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name) ||
                    string.IsNullOrWhiteSpace(Mail) ||
                    string.IsNullOrWhiteSpace(Phone)) { return false; }
                return !IsBusy;
            }
        }
        public bool ChangePasswordVisible
        {
            get
            {
                if (IsBusy) { return false; }
                if (User == null) { return false; }
                if (string.IsNullOrWhiteSpace(User.IdSocialNetwork)) { return true; }
                return false;
            }
        }

        // Actions
        public ICommand UpdateCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        #endregion

        #region Methods

        public ProfileViewModel(Page page) : base(page)
        {
            User = new UserModel();
            UpdateCommand = new Command(async () =>
            {
                bool result = await RegisterValidations();
                if (!result) { return; }

                IsBusy = true;

                try
                {
                    bool response = await RestService.Instance.UpdateProfileAsync(User);
                    if (response){ await DisplayAlertAsync(MessagesTexts.update_profile_message); }
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
            ChangePasswordCommand = new Command(async () => { await OpenPageAsync(new ChangePasswordView()); });

            LoadUserData();
        }

        async void LoadUserData()
        {
            IsBusy = true;

            try
            {
                User = await RestService.Instance.GetProfileAsync();
                if (User == null)
                {
                    await DisplayAlertAsync(MessagesTexts.error_message);
                    User = new UserModel();
                }
            }
            catch { await DisplayAlertAsync(MessagesTexts.error_message); }

            IsBusy = false;
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

            return true;
        }

        protected override void UpdateAdditionalProperties()
        {
            OnPropertyChanged("UpdateButtonStatus");
            OnPropertyChanged("ChangePasswordVisible");
        }

        #endregion
    }
}
