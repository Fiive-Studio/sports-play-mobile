using FutbolPlay.Views;
using FutbolPlay.WebApi.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FacebookLoginView), typeof(FutbolPlay.Renderer.Views.FacebookLoginRenderer))]

namespace FutbolPlay.Renderer.Views
{
    public class FacebookLoginRenderer : PageRenderer
    {
        bool IsShown;

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!IsShown)
            {
                IsShown = true;

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

                PresentViewController(auth.GetUI(), true, null);
            }
        }
    }
}