using Foundation;
using UIKit;
using System;
using EventKit;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading;
using System.Threading.Tasks;

namespace location2
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		private nint bgThread = -1;

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
					Timer timer = new Timer(ttimerCallback, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(20));
				}).Start();
			}
		}
		private void ttimerCallback(object state)
		{

			InvokeOnMainThread(async () => { await GetReservationVehicleLocations(); });
		}

		private async Task GetReservationVehicleLocations()
		{
			var username = NSUserDefaults.StandardUserDefaults.StringForKey("userName");

			trackSvc.Service1 meServ = new trackSvc.Service1();
			meServ = new location2.trackSvc.Service1();

			var eventsData = meServ.getUserCalendarFuture(username);

			App.Current.EventStore.RequestAccess(EKEntityType.Event,
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


