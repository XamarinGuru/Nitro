using Foundation;
using UIKit;
using System;
using EventKit;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace location2
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		private nint bgThread = -1;

		private string username;

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			// Code to start the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
			#endif

			return true;
		}

		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground(UIApplication application)
		{
			var username = NSUserDefaults.StandardUserDefaults.StringForKey("userName");
			if (username == null)
				return;
			
			if (bgThread == -1)
			{
				bgThread = UIApplication.SharedApplication.BeginBackgroundTask(() => { });
				new Task(() =>
				{
					Timer timer = new Timer(ttimerCallback, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60*1));
				}).Start();
			}
		}
		private void ttimerCallback(object state)
		{

			InvokeOnMainThread(async () => { await UpdateCalendar(); });
		}

		private async Task UpdateCalendar()
		{
			username = NSUserDefaults.StandardUserDefaults.StringForKey("userName");

			trackSvc.Service1 meServ = new trackSvc.Service1();
			meServ = new location2.trackSvc.Service1();

			//username = "droidusername";
			var pastEvents = "";//meServ.getUserCalendarPast(username);
			var todayEvents = meServ.getUserCalendarToday(username);
			var futureEvents = meServ.getUserCalendarFuture(username);

			App.Current.EventStore.RequestAccess(EKEntityType.Event,
				(bool granted, NSError e) =>
				{
					InvokeOnMainThread(() =>
					{
						if (granted)
							AddEvents(pastEvents, todayEvents, futureEvents);
						else
							new UIAlertView("Access Denied", "User Denied Access to Calendars/Reminders", null, "ok", null).Show();
					});
				});

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
							var tmp = pEvent.StartDate.Compare(startString);
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
				//AddEventsToNitroCalendar(nitroCalendar, pastEvents);
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

				var startDate = Convert.ToDateTime(eventData["start"].ToString());
				var endDate = Convert.ToDateTime(eventData["end"].ToString());

				DateTime now = DateTime.Now;
				DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
				DateTime startDay = new DateTime(startDate.Year, startDate.Month, startDate.Day, (startDate.Hour - 1), startDate.Minute, startDate.Second);
				var deltaSec = (startDay - startNow).TotalSeconds;
				if (deltaSec < 0)
					continue;

				var tmpStart = startDate.AddHours(-2);
				newEvent.StartDate = ConvertDateTimeToNSDate(tmpStart);
				var tmpEnd = endDate.AddHours(-2);
				newEvent.EndDate = ConvertDateTimeToNSDate(tmpEnd);
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

				//var encodedTitle = System.Web.HttpUtility.UrlEncode(eventData["title"].ToString());
				//var strDate = String.Format("{0:dd-MM-yyyy hh:mm:ss}", startDate);
				//var encodedDate = System.Web.HttpUtility.UrlEncode(strDate);
				//var encodedEventURL = "http://go-heja.com/nitro/calenPage.php?name=" + encodedTitle + "&startdate=" + encodedDate + "&user=" + username;
				////new UIAlertView("encoded url", encodedEventURL, null, "ok", null).Show();
				//var uri = new Uri(encodedEventURL);

				//newEvent.Url = uri;


				var encodedTitle = System.Web.HttpUtility.UrlEncode(eventData["title"].ToString());

				var urlDate = newEvent.StartDate;
				var strDate = String.Format("{0:dd-MM-yyyy hh:mm:ss}", startDate);
				var encodedDate = System.Web.HttpUtility.UrlEncode(strDate);
				var encodedEventURL = "http://go-heja.com/nitro/calenPage.php?name=" + encodedTitle + "&startdate=" + encodedDate + "&user=" + username;

				//var escapedBundlePath = Uri.EscapeUriString(NSBundle.MainBundle.BundlePath);
				var xxx = System.Web.HttpUtility.UrlEncode(encodedEventURL);
				//var myUrl = "Path/To/File/Test.html?var1=hello&var2=world";
				//var nsUrl = new NSUrl(Path.Combine(escapedBundlePath, encodedEventURL));

				//new UIAlertView("encoded date", encodedEventURL, null, "ok", null).Show();
				//var uri = new Uri(encodedEventURL);

				newEvent.Url = new NSUrl(xxx); ;

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
				(date - newDate).TotalSeconds + 3600);
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate(UIApplication application)
		{
			if (bgThread != -1)
			{
				UIApplication.SharedApplication.EndBackgroundTask(bgThread);
			}
		}
	}

}


