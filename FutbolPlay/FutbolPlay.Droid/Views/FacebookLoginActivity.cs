using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using FutbolPlay.Views;
using Xamarin.Auth;
using Newtonsoft.Json.Linq;
using FutbolPlay.Droid.Services;
using FutbolPlay.WebApi.Model;
using Newtonsoft.Json;

[assembly: ExportRenderer(typeof(FacebookLoginView), typeof(FutbolPlay.Droid.Views.FacebookLoginActivity))]

namespace FutbolPlay.Droid.Views
{
    [Activity(Label = "LoginPageRenderer")]
    public class FacebookLoginActivity : PageRenderer
    {
        public FacebookLoginActivity()
        {
            var activity = this.Context as Activity;
            
            var auth = new OAuth2Authenticator(
                clientId: FacebookModel.ClientId, // your OAuth2 client id
                scope: FacebookModel.Scope, // the scopes for the particular API you're accessing, delimited by "+" symbols
                authorizeUrl: new Uri(FacebookModel.AuthorizeUrl),
                redirectUrl: new Uri(FacebookModel.RedirectUrl));

            auth.Completed += async (sender, eventArgs) =>
            {
                if (eventArgs.IsAuthenticated)
                {
                    //var accessToken = eventArgs.Account.Properties["access_token"].ToString();
                    //var expiresIn = Convert.ToDouble(eventArgs.Account.Properties["expires_in"]);
                    //var expiryDate = DateTime.Now + TimeSpan.FromSeconds(expiresIn);

                    var request = new OAuth2Request("GET", new Uri(FacebookModel.UrlQuery), null, eventArgs.Account);
                    var response = await request.GetResponseAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        JObject responseModel = JsonConvert.DeserializeObject<JObject>(response.GetResponseText());

                        UserModel user = new UserModel
                        {
                            IdSocialNetwork = (string)responseModel["id"],
                            Name = (string)responseModel["name"],
                            Mail = (string)responseModel["email"]
                        };

                        App.RegisterSocialNetworkUserAsync(user);
                        return;
                    }
                }

                App.NavigateToHomeAsync();
            };

            activity.StartActivity(auth.GetUI(activity));
        }
    }
}