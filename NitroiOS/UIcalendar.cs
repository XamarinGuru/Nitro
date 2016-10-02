using Foundation;
using System;
using UIKit;
using EventKit;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace location2
{
	partial class UIcalendar : PageContentViewController
	{
		private string userName;
		public UIcalendar(IntPtr handle) : base(handle)
		{
		}
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

				userName = athData[4].ToString();
				var eventsData = meServ.getUserCalendarFuture(userName);

				RequestCalendarAccess(EKEntityType.Event, eventsData);
			}
		}
		protected void RequestCalendarAccess(EKEntityType type, string eventsData)
		{
			App.Current.EventStore.RequestAccess(type,
				(bool granted, NSError e) =>
				{
					InvokeOnMainThread(() =>
					{
						if (granted)
							AddEvents(eventsData);
						else
							new UIAlertView("Access Denied", "User Denied Access to Calendars/Reminders", null, "ok", null).Show();
					});
				});
		}
		private void AddEvents(string eventsJson)
		{
			if (eventsJson == null || eventsJson == "")
				return;

			NSError error;

			////remove existing events
			var calendars = App.Current.EventStore.GetCalendars(EKEntityType.Event);
			foreach (var calendar in calendars)
			{
				if (calendar.Title.Equals("Nitro Calendar"))
					App.Current.EventStore.RemoveCalendar(calendar, true, out error);
			}

			// build out an MT.D list of all the calendars, we show the calendar title
			// as well as the source (where the calendar is pulled from, like iCloud, local
			// exchange, etc.)


			var nitroCalendar = EKCalendar.Create(EKEntityType.Event, App.Current.EventStore);
			EKSource nitroSource = null;




			foreach (EKSource source in App.Current.EventStore.Sources)
			{
				if (source.SourceType == EKSourceType.CalDav && source.Title =="iCloud")
				{
					nitroSource = source;
					break;
				}
			}

			if (nitroSource == null)
			{
				foreach (EKSource source in App.Current.EventStore.Sources)
				{
					if (source.SourceType == EKSourceType.Local)
					{
						nitroSource = source;
						break;
					}
				}
			}

			if (nitroSource == null)
				return;

			nitroCalendar.Title = "Nitro Calendar";
			nitroCalendar.Source = nitroSource;

			App.Current.EventStore.SaveCalendar(nitroCalendar, true, out error);

			eventsJson = eventsJson.Replace("ObjectId(\"", "\"");
			eventsJson = eventsJson.Replace(" ISODate(\"", "\"");
			eventsJson = eventsJson.Replace("\")", "\"");
			var eventsData = JArray.Parse(eventsJson);

			//test event
			EKEvent newEvent = EKEvent.FromStore(App.Current.EventStore);
			newEvent.Title = "xxx";
			newEvent.Calendar = nitroCalendar;
			newEvent.StartDate = ConvertDateTimeToNSDate(DateTime.Now);
			newEvent.EndDate = ConvertDateTimeToNSDate(DateTime.Now.AddHours(2));

			// save the event
			NSError e;
			App.Current.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out e);

			//foreach (var eventJson in eventsData)
			//{
			//	var eventData = JObject.FromObject(eventJson);

			//	EKEvent newEvent = EKEvent.FromStore(App.Current.EventStore);

			//	var startDate = DateTime.Parse(eventData["start"].ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);//Convert.ToDateTime(eventData["start"].ToString());
			//	var endDate = Convert.ToDateTime(eventData["end"].ToString());

			//	//newEvent.EventIdentifier = eventData["_id"].ToString();
			//	newEvent.AddAlarm(EKAlarm.FromDate(ConvertDateTimeToNSDate(startDate.AddMinutes(5))));
			//	newEvent.StartDate = ConvertDateTimeToNSDate(startDate);
			//	newEvent.EndDate = ConvertDateTimeToNSDate(endDate);
			//	newEvent.Title = eventData["title"].ToString();
			//	newEvent.Notes = "distance : " + eventData["distance"].ToString() + Environment.NewLine +
			//					"type : " + eventData["type"].ToString() + Environment.NewLine +
			//					"notes : " + eventData["notes"].ToString() + Environment.NewLine +
			//					"programName : " + eventData["programName"].ToString() + Environment.NewLine +
			//					"programStart : " + eventData["programStart"].ToString() + Environment.NewLine +
			//					"programEnd : " + eventData["programEnd"].ToString() + Environment.NewLine +
			//					"durHrs : " + eventData["durHrs"].ToString() + Environment.NewLine +
			//					"durMin : " + eventData["durMin"].ToString() + Environment.NewLine +
			//					"tss : " + eventData["tss"].ToString() + Environment.NewLine +
			//					"hb : " + eventData["hb"].ToString() + Environment.NewLine;

			//	//var structuredLocation = new EKStructuredLocation();
			//	//structuredLocation.Title = "my location";
			//	//structuredLocation.GeoLocation = new CoreLocation.CLLocation(100, 100);

			//	//newEvent.StructuredLocation = structuredLocation;
			//	var eventName = eventData["title"].ToString().Replace(" ", "%20");
			//	var strEventURL = "http://go-heja.com/nitro/existEventByName.php?eventSelectedName=" + eventName;
			//	//var encordedURL = System.Web.HttpUtility.UrlPathEncode(strEventURL);
			//	var uri = new Uri(strEventURL);
			//	newEvent.Url = new NSUrl(strEventURL);

			//	newEvent.Calendar = nitroCalendar;

			//	// save the event
			//	NSError e;
			//	App.Current.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out e);
			//}
		}

		public NSDate ConvertDateTimeToNSDate(DateTime date)
		{
			DateTime newDate = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - newDate).TotalSeconds);
		}
	}
}
