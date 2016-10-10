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

using Android.Provider;

using Java.Util;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//by Afroz date 31/8/2016
namespace goheja
{
    public class FragmentCalendar : Android.Support.V4.App.Fragment
    {
		private static Intent serviceIntent = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.calendar, container, false);
            var webView = view.FindViewById<WebView>(Resource.Id.webViewCalen);

			if (serviceIntent == null)
			{
				serviceIntent = new Intent(this.Activity, typeof(BackgroundService));
				this.Activity.StartService(serviceIntent);
			}
           
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.AllowContentAccess = true;
            webView.Settings.EnableSmoothTransition();
            webView.Settings.LoadsImagesAutomatically = true;
            webView.Settings.SetGeolocationEnabled(true);
            webView.SetWebViewClient(new WebViewClient());

            var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
            string nickName = contextPref.GetString("storedUserName", "");
			webView.ClearCache(true);
			webView.ClearHistory();
            webView.LoadUrl("http://go-heja.com/nitro/mobda.php?userNickName=" + nickName + "&userId=" + contextPref.GetString("storedAthId", ""));

			try
			{
				trackSvc.Service1 meServ = new trackSvc.Service1();
				meServ = new trackSvc.Service1();
			}
			catch(Exception e)
			{
				Toast.MakeText(this.Activity, e.Message, ToastLength.Short);
			}
            return view;
        }
    }
}
//end by Afroz date 31/8/2016