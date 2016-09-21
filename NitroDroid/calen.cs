
using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using System;


namespace goheja
{
	[Activity (Label = "Calendar and reports")]			
	public class calen : Activity
	{
		ISharedPreferences contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
		WebView wv1;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			string _deviceId = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
			trackSvc.Service1 test = new trackSvc.Service1();
			var contextEdit = contextPref.Edit();
			base.OnCreate (savedInstanceState);
			string[] athData = test.getAthDataByDeviceId(_deviceId);
			contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			contextEdit = contextPref.Edit();

			contextEdit.PutString("storedFirstName", athData[0].ToString());
			contextEdit.PutString("storedAthId", athData[2].ToString());
			contextEdit.PutString("storedLastName", athData[1].ToString());
			contextEdit.PutString("storedCountry", athData[3].ToString());
			contextEdit.PutString("storedUserName", athData[4].ToString());
			contextEdit.PutString("storedPsw", athData[5].ToString());

			string deviceId = "0";
			try
			{
			    deviceId = test.getListedDeviceId(Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
			}
			catch (Exception err)
			{
			    var builder = new AlertDialog.Builder(this);
			    builder.SetTitle("Nitro service is not available");
			    builder.SetMessage("Oops!Service not available... Pls try again later");
			    builder.SetCancelable(false);
			    builder.SetPositiveButton("OK", delegate { Finish(); });
			    builder.Show();
			    return;
			}

			if (deviceId == "0")
			{
			    var activity2 = new Intent(this, typeof(listingActivity));
			    activity2.PutExtra("MyData", "Data from Activity1");
			    StartActivity(activity2);
			}
			else
			{
			}

			contextEdit.PutFloat("lastAlt", 0f);
			contextEdit.PutFloat("gainAlt", 0f);
			contextEdit.PutFloat("distance", 0f);
			contextEdit.PutString("prevLoc", "");
			SetContentView (Resource.Layout.calen);
			showPage ();
		}
		protected override void OnStop()
		{
			base.OnStop();
		}

		protected override void OnResume()
		{
			base.OnResume();
		}
		protected override void OnPause()
		{
			base.OnPause();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		private void showPage()
		{
			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			wv1 = FindViewById<WebView> (Resource.Id.webViewCalen);
			wv1.Settings.JavaScriptEnabled = true;
			wv1.Settings.AllowContentAccess = true;
			wv1.Settings.EnableSmoothTransition ();
			wv1.Settings.LoadsImagesAutomatically = true;
			wv1.Settings.SetGeolocationEnabled (true);

			wv1.Settings.SetGeolocationEnabled (true);
			wv1.SetWebViewClient(new WebViewClient());
			string nickName = contextPref.GetString ("storedUserName", "");
			string id = contextPref.GetString ("storedAthId", "");
			wv1.LoadUrl ("http://go-heja.com/nitro/mobda.php?userNickName=" + nickName+"&userId="+id);
		}
	}
}

