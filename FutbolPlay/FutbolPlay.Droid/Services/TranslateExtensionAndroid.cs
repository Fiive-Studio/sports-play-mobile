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
using System.Globalization;
using FutbolPlay.Services;
using Xamarin.Forms;
using System.Threading;

[assembly: Dependency(typeof(FutbolPlay.Droid.Services.TranslateExtensionAndroid))]


namespace FutbolPlay.Droid.Services
{
    public class TranslateExtensionAndroid : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace("_", "-");

            return new CultureInfo(netLanguage);
        }

        public void SetLocale()
        {
            var ci = GetCurrentCultureInfo();

            NumberFormatInfo info3 = new NumberFormatInfo();
            info3.PositiveSign = "+";
            info3.NegativeSign = "-";
            info3.NumberDecimalSeparator = ".";
            info3.NumberDecimalDigits = 3;
            info3.NumberGroupSeparator = ",";
            info3.PercentDecimalSeparator = ".";
            info3.PercentDecimalDigits = 3;
            info3.PercentGroupSeparator = ",";
            info3.PercentSymbol = "%";
            info3.CurrencyDecimalSeparator = ".";
            info3.CurrencyDecimalDigits = 3;
            info3.CurrencyGroupSeparator = ",";
            info3.CurrencySymbol = "$";
            ci.NumberFormat = info3;

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}