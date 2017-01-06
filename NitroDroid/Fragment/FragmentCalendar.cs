using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using PortableLibrary;

namespace goheja
{
    public class FragmentCalendar : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			var view = inflater.Inflate(Resource.Layout.fCalendar, container, false);
            var webView = view.FindViewById<WebView>(Resource.Id.webViewCalen);
           
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.AllowContentAccess = true;
            webView.Settings.EnableSmoothTransition();
            webView.Settings.LoadsImagesAutomatically = true;
            webView.Settings.SetGeolocationEnabled(true);
            webView.SetWebViewClient(new WebViewClient());

			webView.ClearCache(true);
			webView.ClearHistory();

			var url = string.Format(Constants.CALENDAR_URL, AppSettings.Username, AppSettings.UserID);
			webView.LoadUrl(url);

            return view;
        }
    }
}
