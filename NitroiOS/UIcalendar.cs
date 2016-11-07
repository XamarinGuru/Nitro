using Foundation;
using System;
using System.IO;
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
		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			App.Current.EventStore.RequestAccess(EKEntityType.Event,
				(bool granted, NSError e) =>
				{
					InvokeOnMainThread(() =>
					{
						if (granted)
						{ }

						else
							new UIAlertView("Access Denied", "User Denied Access to Calendars/Reminders", null, "ok", null).Show();
					});
				});



			GetUserData();

			var url="";
			string id=NSUserDefaults.StandardUserDefaults.StringForKey ( "id");
			userName=NSUserDefaults.StandardUserDefaults.StringForKey ("userName");
			url = "http://go-heja.com/nitro/mobda.php?userNickName=" + userName + "&userId=" + id; // NOTE: https secure request
			//url = "http://go-heja.com/nitro/mobda.php?userNickName=Goheja%20user&userId=57a19ffd16528a08e0d3f199#pageC";

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


			try
			{
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

					////userName = "five";
					//var pastEvents = meServ.getUserCalendarPast(userName);
					////var pastEvents = "";
					//var todayEvents = meServ.getUserCalendarToday(userName);
					//var futureEvents = meServ.getUserCalendarFuture(userName);

					//App.Current.EventStore.RequestAccess(EKEntityType.Event,
					//	(bool granted, NSError e) =>
					//	{
					//		InvokeOnMainThread(() =>
					//		{
					//			if (granted)
					//				AddEvents(pastEvents, todayEvents, futureEvents);
					//			else
					//				new UIAlertView("Access Denied", "User Denied Access to Calendars/Reminders", null, "ok", null).Show();
					//		});
					//	});
				}
			}
			catch(Exception e)
			{
				new UIAlertView(null, e.Message, null, "OK", null).Show();
			}
		}

		private void AddEvents(string pastEvents, string todayEvents, string futureEvents)
		{
			try
			{
				NSError error;

				EKCalendar nitroCalendar = null;
				////remove existing events
				var calendars = App.Current.EventStore.GetCalendars(EKEntityType.Event);
				foreach (var calendar in calendars)
				{
					if (calendar.Title == "Nitro Events")
					{
						nitroCalendar = calendar;

						EKCalendar[] calendarArray = new EKCalendar[1];
						calendarArray[0] = calendar;
						NSPredicate pEvents = App.Current.EventStore.PredicateForEvents(NSDate.Now.AddSeconds(-(3600 * 10000)), NSDate.Now.AddSeconds(3600 * 10000), calendarArray);
						EKEvent[] allEvents = App.Current.EventStore.EventsMatching(pEvents);
						if (allEvents == null)
							continue;
						foreach (var pEvent in allEvents)
						{
							NSError pE;
							DateTime now = DateTime.Now;
							DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
							var startString = ConvertDateTimeToNSDate(startNow);
							if (pEvent.StartDate.Compare(startString) == NSComparisonResult.Descending)
								App.Current.EventStore.RemoveEvent(pEvent, EKSpan.ThisEvent, true, out pE);
						}
					}
				}

				if (nitroCalendar == null)
				{
					nitroCalendar = EKCalendar.Create(EKEntityType.Event, App.Current.EventStore);
					EKSource nitroSource = null;

					foreach (EKSource source in App.Current.EventStore.Sources)
					{
						if (source.SourceType == EKSourceType.CalDav && source.Title == "iCloud")
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

					nitroCalendar.Title = "Nitro Events";
					nitroCalendar.Source = nitroSource;
				}


				App.Current.EventStore.SaveCalendar(nitroCalendar, true, out error);
				AddEventsToNitroCalendar(nitroCalendar, pastEvents);
				AddEventsToNitroCalendar(nitroCalendar, todayEvents);
				AddEventsToNitroCalendar(nitroCalendar, futureEvents);
			}
			catch (Exception e)
			{
				new UIAlertView("add events process", e.Message, null, "ok", null).Show();
			}

		}

		private void AddEventsToNitroCalendar(EKCalendar nitroCalendar, string eventsJson)
		{
			if (eventsJson == null || eventsJson == "" || eventsJson == "[]")
				return;

			eventsJson = eventsJson.Replace("ObjectId(\"", "\"");
			eventsJson = eventsJson.Replace(" ISODate(\"", "\"");
			eventsJson = eventsJson.Replace("\")", "\"");
			var eventsData = JArray.Parse(eventsJson);

			foreach (var eventJson in eventsData)
			{
				var eventData = JObject.FromObject(eventJson);

				EKEvent newEvent = EKEvent.FromStore(App.Current.EventStore);

				var startDate = DateTime.Parse(eventData["start"].ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);//Convert.ToDateTime(eventData["start"].ToString());
				var endDate = Convert.ToDateTime(eventData["end"].ToString());

				//newEvent.AddAlarm(EKAlarm.FromDate(ConvertDateTimeToNSDate(startDate.AddMinutes(5))));
				newEvent.StartDate = ConvertDateTimeToNSDate(startDate.AddMinutes(-60));
				newEvent.EndDate = ConvertDateTimeToNSDate(endDate.AddMinutes(-60));
				newEvent.Title = eventData["title"].ToString();

				string eventDescription = eventData["eventData"].ToString();
				eventDescription = eventDescription.Replace("<textarea id =\"genData\" class=\"generalData\" name=\"pDesc\"  placeholder=\"Right here coach\" maxlength=\"1000\">", "");
				eventDescription = eventDescription.Replace("</textarea><br/>", "");

				string[] arryEventDes = eventDescription.Split(new char[] { '~' });

				for (var i = 0; i < arryEventDes.Length; i++)
				{
					newEvent.Notes += arryEventDes[i].ToString() + Environment.NewLine;
				}

				var strDistance = eventData["distance"].ToString();
				var floatDistance = strDistance == "" ? 0 : float.Parse(strDistance);
				var b = Math.Truncate(floatDistance * 100);
				var c = b / 100;
				var formattedDistance = c.ToString("F2");

				var durMin = eventData["durMin"].ToString() == "" ? 0 : int.Parse(eventData["durMin"].ToString());
				var durHrs = eventData["durHrs"].ToString() == "" ? 0 : int.Parse(eventData["durHrs"].ToString());
				var pHrs = durMin / 60;
				durHrs = durHrs + pHrs;
				durMin = durMin % 60;

				var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

				newEvent.Notes += Environment.NewLine + "Planned HB : " + eventData["hb"].ToString() + Environment.NewLine +
								"Planned TSS : " + eventData["tss"].ToString() + Environment.NewLine +
								"Planned distance : " + formattedDistance + "KM" + Environment.NewLine +
								"Duration : " + strDuration + Environment.NewLine;

				//var structuredLocation = new EKStructuredLocation();
				//structuredLocation.Title = "my location";
				//structuredLocation.GeoLocation = new CoreLocation.CLLocation(100, 100);

				var encodedTitle = System.Web.HttpUtility.UrlEncode(eventData["title"].ToString());

				var urlDate = newEvent.StartDate;
				var strDate = String.Format("{0:dd-MM-yyyy hh:mm:ss}", startDate);
				var encodedDate = System.Web.HttpUtility.UrlEncode(strDate);
				var encodedEventURL = "http://go-heja.com/nitro/calenPage.php?name=" + "test event" + "&startdate=" + encodedDate + "&user=" + userName;

				//var escapedBundlePath = Uri.EscapeUriString(NSBundle.MainBundle.BundlePath);
				var xxx = System.Web.HttpUtility.UrlEncode(encodedEventURL);
				//var myUrl = "Path/To/File/Test.html?var1=hello&var2=world";
				//var nsUrl = new NSUrl(Path.Combine(escapedBundlePath, encodedEventURL));

				//new UIAlertView("encoded date", encodedEventURL, null, "ok", null).Show();
				//var uri = new Uri(encodedEventURL);

				newEvent.Url = new NSUrl(xxx);

				EKAlarm[] alarmsArray = new EKAlarm[2];
				alarmsArray[0] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 45)));
				alarmsArray[1] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 60 * 12)));
				newEvent.Alarms = alarmsArray;

				newEvent.Calendar = nitroCalendar;

				NSError e;
				App.Current.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out e);
			}
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
