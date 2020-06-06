using FutbolPlay.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FutbolPlay.UWP.Services.TranslateExtensionUWP))]

namespace FutbolPlay.UWP.Services
{
    public class TranslateExtensionUWP : ILocalize
    {
        public void SetLocale() { }
        public CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentCulture;
        }
    }
}
