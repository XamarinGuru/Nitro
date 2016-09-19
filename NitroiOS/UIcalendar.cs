using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace location2
{
	partial class UIcalendar : PageContentViewController
	{
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			GetUserData();

			var url="";
			string id=NSUserDefaults.StandardUserDefaults.StringForKey ( "id");
			string userName=NSUserDefaults.StandardUserDefaults.StringForKey ("userName");
			url = "http://go-heja.com/nitro/mobda.php?userNickName=" + userName + "&userId=" + id; // NOTE: https secure request
			//if (NSUserDefaults.StandardUserDefaults.StringForKey ( "source")=="calen")
			//{
			//	url = "http://go-heja.com/nitro/mobda.php?userNickName=" + userName + "&userId=" + id; // NOTE: https secure request
			//}
			//else
			//{
			//	url = "http://go-heja.com/nitro/profile.php?txt=" + userName+ "&userId=" + id; // NOTE: https secure request
			//}

		   calendarWebView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

		}
		private void GetUserData()
		{
			string deviceUDID = NSUserDefaults.StandardUserDefaults.StringForKey("deviceId");

			UIApplication.SharedApplication.IdleTimerDisabled = true;
			///check if device exists in service
			/// if not - list the user using vcListing
			//check the device id
			trackSvc.Service1 meServ = new trackSvc.Service1();
			meServ = new location2.trackSvc.Service1();

			string deviceId;
			try
			{
				deviceId = meServ.getListedDeviceId(deviceUDID);
			}
			catch
			{
				new UIAlertView(null, "You are not connected to Nitro services...", null, "OK", null).Show();
				deviceId = "tempDeviceId";
			}

			if (deviceId == "0")
			{
				//vcListing controller = this.Storyboard.InstantiateViewController("vcListing") as vcListing;
				//this.NavigationController.PushViewController(controller, true);
			}
			else {
				string[] athData = meServ.getAthDataByDeviceId(NSUserDefaults.StandardUserDefaults.StringForKey("deviceId"));
				if (athData == null) athData = meServ.getAthDataByDeviceId(deviceUDID);
				NSUserDefaults.StandardUserDefaults.SetString(deviceUDID, "deviceId");
				NSUserDefaults.StandardUserDefaults.SetString(athData[0].ToString(), "firstName");
				NSUserDefaults.StandardUserDefaults.SetString(athData[1].ToString(), "lastName");
				NSUserDefaults.StandardUserDefaults.SetString(athData[2].ToString(), "id");
				NSUserDefaults.StandardUserDefaults.SetString(athData[3].ToString(), "country");
				NSUserDefaults.StandardUserDefaults.SetString(athData[4].ToString(), "userName");
				NSUserDefaults.StandardUserDefaults.SetString(athData[5].ToString(), "password");
			}
		}
		public UIcalendar (IntPtr handle) : base (handle)
		{
		}
	}
}
