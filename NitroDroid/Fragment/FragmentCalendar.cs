using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace goheja
{
    public class FragmentCalendar : Android.Support.V4.App.Fragment
    {
		private static Intent serviceIntent = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			var view = inflater.Inflate(Resource.Layout.fCalendar, container, false);
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
			string athID = contextPref.GetString("storedAthId", "");
			webView.ClearCache(true);
			webView.ClearHistory();
            webView.LoadUrl("http://go-heja.com/nitro/mobda.php?userNickName=" + nickName + "&userId=" + athID);

            return view;
        }
    }
}
