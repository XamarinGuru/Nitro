using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;

//by Afroz date 31/8/2016
namespace goheja
{
    public class FragmentCalendar : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.calendar, container, false);
            var webView = view.FindViewById<WebView>(Resource.Id.webViewCalen);
           
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.AllowContentAccess = true;
            webView.Settings.EnableSmoothTransition();
            webView.Settings.LoadsImagesAutomatically = true;
            webView.Settings.SetGeolocationEnabled(true);
            webView.SetWebViewClient(new WebViewClient());

            var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
            string nickName = contextPref.GetString("storedUserName", "");
            webView.LoadUrl("http://go-heja.com/nitro/mobda.php?userNickName=" + nickName + "&userId=" + contextPref.GetString("storedAthId", ""));
            return view;
        }
    }
}
//end by Afroz date 31/8/2016