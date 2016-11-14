
using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
namespace goheja
{
	[Activity (Label = "Profile")]			
	public class ProfileWebViewActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.ProfileWebViewActivity);
			WebView wv1 = FindViewById<WebView>(Resource.Id.webViewProfile);
			wv1.Settings.JavaScriptEnabled = true;
			wv1.SetWebViewClient(new WebViewClient());

			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			string nickName = contextPref.GetString("storedUserName", "");
			string id = contextPref.GetString("storedAthId", "");
			wv1.LoadUrl("http://go-heja.com/nitro/profile.php?txt=" + nickName + "&userId=" + id);
		}
	}
}

