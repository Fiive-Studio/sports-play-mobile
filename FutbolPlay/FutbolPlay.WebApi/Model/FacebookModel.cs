using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutbolPlay.WebApi.Model
{
    public static class FacebookModel
    {
        public static string ClientId { get { return ""; } }
        public static string Scope { get { return "email"; } }
        public static string AuthorizeUrl { get { return "https://m.facebook.com/dialog/oauth/"; } }
        public static string RedirectUrl { get { return "http://www.facebook.com/connect/login_success.html"; } }
        public static string UrlQuery { get { return "https://graph.facebook.com/me?fields=email,name"; } }
    }
}
