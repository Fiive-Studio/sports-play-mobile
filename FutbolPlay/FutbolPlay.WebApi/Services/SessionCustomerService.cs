using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace FutbolPlay.WebApi.Services
{
    public class SessionCustomerService
    {
        #region Account

        readonly static string _servideId = "FutbolPlayAdminAppAuth";

        public static AuthCustomerModel Account { get; set; }

        public static void SaveAccount(AuthCustomerModel auth)
        {
            Account account = new Account();
            account.Properties.Add("Token", auth.Token);
            account.Properties.Add("ExpiresIn", auth.ExpiresIn);
            account.Properties.Add("IdUser", auth.IdUser);
            account.Properties.Add("UserName", auth.UserName);
            account.Properties.Add("IdPlace", auth.IdPlace);

            try
            {
                AccountStore.Create().Save(account, _servideId);
            }
            catch (Exception e)
            {
                string exx = e.ToString();
            }

            auth.ExpirationDate = DateTime.Now + TimeSpan.FromSeconds(Convert.ToDouble(auth.ExpiresIn));
            Account = auth;
        }

        public static async Task<bool> ValidateLoginAsync()
        {
            AccountStore accountStore = AccountStore.Create();
            Account account = accountStore.FindAccountsForService(_servideId).SingleOrDefault();

            if (account != null)
            {
                bool result = await RestService.Instance.ValidateTokenCustomerAsync(account.Properties["Token"], account.Properties["IdPlace"]);
                if (result)
                {
                    Account = new AuthCustomerModel
                    {
                        Token = account.Properties["Token"],
                        ExpiresIn = account.Properties["ExpiresIn"],
                        IdUser = account.Properties["IdUser"],
                        UserName = account.Properties["UserName"],
                        IdPlace = account.Properties["IdPlace"],
                        ExpirationDate = DateTime.Now + TimeSpan.FromSeconds(Convert.ToDouble(account.Properties["ExpiresIn"]))
                    };

                    Place = await RestService.Instance.GetPlaceAsync(Convert.ToInt32(Account.IdPlace));
                    if (Place == null)
                    {
                        DeleteAccount();
                        return false;
                    }
                    else { Place.HasMutiple = await RestService.Instance.ValidateIfPlaceHasMultiples(Place.Id); }
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

        #region Place Cache

        public static PlaceModel Place { get; set; }

        #endregion
    }
}
