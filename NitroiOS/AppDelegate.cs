﻿using Foundation;
using UIKit;
using System;
using System.Threading;
using System.Threading.Tasks;
using EventKit;
using Newtonsoft.Json.Linq;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp;
using PortableLibrary;

namespace location2
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		private nint bgThread = -1;

		public BaseViewController baseVC;

		EKCalendar nitroCalendar = null;

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
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
			if (AppSettings.Username == null || baseVC == null)
				return;

			App.Current.EventStore.RequestAccess(EKEntityType.Event,
				(bool granted, NSError e) =>
				{
					InvokeOnMainThread(() =>
					{
						if (granted)
							UpdateCalendarTimer();
					});
				});
		}

		private void UpdateCalendarTimer()
		{
			if (bgThread == -1)
			{
				bgThread = UIApplication.SharedApplication.BeginBackgroundTask(() => { });
				new Task(() => { new Timer(UpdateCalendar, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60 * 30)); }).Start();
			}
		}

		private void UpdateCalendar(object state)
		{
			InvokeOnMainThread(() => { AddNitroCalendarToDevice(); });
		}

		private void AddNitroCalendarToDevice()
		{
			try
			{
				NSError error;

				////remove existing descending events from now in "Nitro Events" calendar of device.
				var calendars = App.Current.EventStore.GetCalendars(EKEntityType.Event);
				foreach (var calendar in calendars)
				{
					if (calendar.Title == Constants.CALENDAR_TITLE)
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
							var startString = baseVC.ConvertDateTimeToNSDate(startNow);
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
					{
						return;
					}
					nitroCalendar.Title = Constants.CALENDAR_TITLE;
					nitroCalendar.Source = nitroSource;
				}

				App.Current.EventStore.SaveCalendar(nitroCalendar, true, out error);

				if (error == null)
					AddEvents();
			}
			catch (Exception e)
			{
				new UIAlertView("add events process", e.Message, null, "ok", null).Show();
			}
		}

		private void AddEvents()
		{
			//var pastEvents = baseVC.GetPastEvents();
			var todayEvents = baseVC.GetTodayEvents();
			var futureEvents = baseVC.GetFutureEvents();

			//AddEventsToNitroCalendar(pastEvents);
			AddEventsToNitroCalendar(todayEvents);
			AddEventsToNitroCalendar(futureEvents);
		}

		private void AddEventsToNitroCalendar(JArray eventsData)
		{
			if (nitroCalendar == null || eventsData == null)
				return;

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
				newEvent.StartDate = baseVC.ConvertDateTimeToNSDate(tmpStart);
				var tmpEnd = endDate.AddHours(-2);
				newEvent.EndDate = baseVC.ConvertDateTimeToNSDate(tmpEnd);
				newEvent.Title = eventData["title"].ToString();

				string eventDescription = baseVC.FormatEventDescription(eventData["eventData"].ToString());

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

				var encodedTitle = System.Web.HttpUtility.UrlEncode(eventData["title"].ToString());

				var urlDate = newEvent.StartDate;
				var strDate = String.Format("{0:dd-MM-yyyy hh:mm:ss}", startDate);
				var encodedDate = System.Web.HttpUtility.UrlEncode(strDate);
				var encodedEventURL = String.Format(Constants.EVENT_MAP_URL, encodedTitle, encodedDate, AppSettings.Username);

				newEvent.Url = new NSUrl(System.Web.HttpUtility.UrlEncode(encodedEventURL)); ;

				//add alarm to event
				EKAlarm[] alarmsArray = new EKAlarm[2];
				alarmsArray[0] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 45)));
				alarmsArray[1] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 60 * 12)));
				newEvent.Alarms = alarmsArray;

				newEvent.Calendar = nitroCalendar;

				NSError e;
				App.Current.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out e);
			}
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


