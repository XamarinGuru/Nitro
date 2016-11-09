using Foundation;
using System;
using System.IO;
using UIKit;
using EventKit;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace location2
{
	partial class CalendarViewController : BaseViewController
	{
		public CalendarViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//load calendar from server
			string id = NSUserDefaults.StandardUserDefaults.StringForKey("id");
			string userName = NSUserDefaults.StandardUserDefaults.StringForKey("userName");
			var url = "http://go-heja.com/nitro/mobda.php?userNickName=" + userName + "&userId=" + id;
			calendarWebView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

			//request for accessing device calendar
			App.Current.EventStore.RequestAccess(EKEntityType.Event, (bool granted, NSError e) =>
			{
				InvokeOnMainThread(() =>
				{
					if (!granted)
						ShowMessageBox("Access Denied", "User Denied Access to Calendars/Reminders");
				});
			});

			GetUserData();
		}

		private void GetUserData()
		{
			string deviceUDID = NSUserDefaults.StandardUserDefaults.StringForKey("deviceId");

			UIApplication.SharedApplication.IdleTimerDisabled = true;

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
				ShowMessageBox(null, "You are not connected to Nitro services...");
				deviceId = "tempDeviceId";
			}


			try
			{
				if (deviceId == "0")
				{
					ShowMessageBox(null, "You are not registered to Nitro services...");
				}
				else {
					string[] athData = meServ.getAthDataByDeviceId(deviceUDID);

					if (athData == null) 
						athData = meServ.getAthDataByDeviceId(deviceUDID);

					NSUserDefaults.StandardUserDefaults.SetString(deviceUDID, "deviceId");
					NSUserDefaults.StandardUserDefaults.SetString(athData[0].ToString(), "firstName");
					NSUserDefaults.StandardUserDefaults.SetString(athData[1].ToString(), "lastName");
					NSUserDefaults.StandardUserDefaults.SetString(athData[2].ToString(), "id");
					NSUserDefaults.StandardUserDefaults.SetString(athData[3].ToString(), "country");
					NSUserDefaults.StandardUserDefaults.SetString(athData[4].ToString(), "userName");
					NSUserDefaults.StandardUserDefaults.SetString(athData[5].ToString(), "password");
				}
			}
			catch (Exception e)
			{
				ShowMessageBox(null, e.Message);
			}
		}
	}
}
