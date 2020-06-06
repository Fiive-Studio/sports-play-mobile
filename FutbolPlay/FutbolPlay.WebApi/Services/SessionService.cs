using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace FutbolPlay.WebApi.Services
{
    public class SessionService
    {
        #region Account

        readonly static string _servideId = "FutbolPlayAppAuth";

        public static AuthModel Account { get; set; }

        public static void SaveAccount(AuthModel auth)
        {
            Account account = new Account();
            account.Properties.Add("Token", auth.Token);
            account.Properties.Add("ExpiresIn", auth.ExpiresIn);
            account.Properties.Add("AuthType", auth.AuthType.ToString());
            account.Properties.Add("IdUser", auth.IdUser);
            account.Properties.Add("UserName", auth.UserName);
            account.Properties.Add("UserPhone", auth.UserPhone);

            if (auth.IdSocialNetwork == null) { auth.IdSocialNetwork = string.Empty; }
            account.Properties.Add("IdSocialNetwork", auth.IdSocialNetwork);

            try
            {
                AccountStore.Create().Save(account, _servideId);
            }
            catch (Exception e)
            {
                string exc = e.ToString();
            }

            auth.ExpirationDate = DateTime.Now + TimeSpan.FromSeconds(Convert.ToDouble(auth.ExpiresIn));
            Account = auth;
        }

        public static void UpdateAccount(UserModel user)
        {
            Account account = new Account();
            account.Properties.Add("Token", Account.Token);
            account.Properties.Add("ExpiresIn", Account.ExpiresIn);
            account.Properties.Add("AuthType", Account.AuthType.ToString());
            account.Properties.Add("IdUser", Account.IdUser);
            account.Properties.Add("UserName", user.Name);
            account.Properties.Add("UserPhone", user.Phone);
            account.Properties.Add("IdSocialNetwork", Account.IdSocialNetwork);           

            AccountStore.Create().Save(account, _servideId);
            Account.UserName = user.Name;
            Account.UserPhone = user.Phone;
        }

        public static async Task<bool> ValidateLoginAsync()
        {
            AccountStore accountStore = AccountStore.Create();
            Account account = accountStore.FindAccountsForService(_servideId).SingleOrDefault();

            if (account != null)
            {
                bool result = await RestService.Instance.ValidateTokenAsync(account.Properties["Token"]);
                if (result)
                {
                    Account = new AuthModel
                    {
                        Token = account.Properties["Token"],
                        ExpiresIn = account.Properties["ExpiresIn"],
                        IdUser = account.Properties["IdUser"],
                        UserName = account.Properties["UserName"],
                        UserPhone = account.Properties["UserPhone"],
                        IdSocialNetwork = account.Properties["IdSocialNetwork"],
                        AuthType = (AuthType)Enum.Parse(typeof(AuthType), account.Properties["AuthType"]),
                        ExpirationDate = DateTime.Now + TimeSpan.FromSeconds(Convert.ToDouble(account.Properties["ExpiresIn"]))
                    };
                }

                return result;
            }

            return false;
        }

        public static void DeleteAccount()
        {
            AccountStore accountStore = AccountStore.Create();
            Account account = accountStore.FindAccountsForService(_servideId).SingleOrDefault();
            if (account != null) { accountStore.Delete(account, _servideId); }
        }

        #endregion

        #region Places Cache

        public static List<PlaceModel> Places { get; set; }

        #endregion
    }

    public enum AuthType { FutbolPlayApp, SocialNetwork }
}
