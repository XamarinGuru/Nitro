﻿using Foundation;
using UIKit;
using System;
using EventKit;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;


using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using FireSharp;
using FireSharp.Extensions;




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


		private static FirebaseClient _client;

		public class Todo
		{
			public string actionName { get; set; }
			public string status { get; set; }
		}

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

		public override async void DidEnterBackground(UIApplication application)
		{
			username = NSUserDefaults.StandardUserDefaults.StringForKey("userName");
			if (username == null)
				return;

			#region config firebase
			IFirebaseConfig config = new FirebaseConfig
			{
				AuthSecret = "PtLxxW6zYGZSE3UXmmiFxVCqzNdZOLxLNHdHNixF",
				BasePath = "https://nitro-8cbda.firebaseio.com/"
			};
			_client = new FirebaseClient(config);

			var todo1 = new Todo
			{
				actionName = "entered background mode",
				status = "y"
			};
			await _client.PushAsync("users/"+username, todo1);
			#endregion

			if (bgThread == -1)
			{
				bgThread = UIApplication.SharedApplication.BeginBackgroundTask(() => { });
				new Task(() => { new Timer(UpdateCalendarTimer, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60*30)); }).Start();
			}
		}
		private void UpdateCalendarTimer(object state)
		{
			InvokeOnMainThread(async () => { await UpdateCalendar(); });
		}

		private async Task UpdateCalendar()
		{
			username = NSUserDefaults.StandardUserDefaults.StringForKey("userName");

			trackSvc.Service1 meServ = new trackSvc.Service1();
			meServ = new location2.trackSvc.Service1();

			username = "Gili";
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
					});
				});

		}

		private async void AddEvents(string pastEvents, string todayEvents, string futureEvents)
		{
			try
			{
				NSError error;

				EKCalendar nitroCalendar = null;
				////remove existing descending events from now in "Nitro Events" calendar of device.
				var calendars = App.Current.EventStore.GetCalendars(EKEntityType.Event);
				foreach (var calendar in calendars)
				{
					if (calendar.Title == "Nitro Events")
					{
						var todo1 = new Todo
						{
							actionName = "nitro calendar already exist?",
							status = "y"
						};
						//track existence of "Nitro Calendar" to firebase
						await _client.PushAsync("users/" + username, todo1);

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
					var todo1 = new Todo
					{
						actionName = "nitro calendar already exist?",
						status = "n"
					};
					//track existence of "Nitro Calendar" to firebase
					await _client.PushAsync("users/" + username, todo1);

					nitroCalendar = EKCalendar.Create(EKEntityType.Event, App.Current.EventStore);
					EKSource nitroSource = null;

					foreach (EKSource source in App.Current.EventStore.Sources)
					{
						if (source.SourceType == EKSourceType.CalDav && source.Title == "iCloud")
						{
							var todo2 = new Todo
							{
								actionName = "event store source",
								status = "iCloud"
							};
							//track source type of "Nitro Calendar" to firebase
							await _client.PushAsync("users/" + username, todo2);

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
								var todo3 = new Todo
								{
									actionName = "event store source",
									status = "Local"
								};
								//track source type of "Nitro Calendar" to firebase
								await _client.PushAsync("users/" + username, todo3);

								nitroSource = source;
								break;
							}
						}
					}
					if (nitroSource == null)
					{
						var todo4 = new Todo
						{
							actionName = "event store source",
							status = "null"
						};
						//track source type of "Nitro Calendar" to firebase
						await _client.PushAsync("users/" + username, todo4);

						return;
					}
					nitroCalendar.Title = "Nitro Events";
					nitroCalendar.Source = nitroSource;
				}


				App.Current.EventStore.SaveCalendar(nitroCalendar, true, out error);

				var todo5 = new Todo
				{
					actionName = "save calendar",
					status = error==null?"null":"error"
				};
				//track source saving status of "Nitro Calendar" to firebase
				await _client.PushAsync("users/" + username, todo5);

				//AddEventsToNitroCalendar(nitroCalendar, pastEvents);
				AddEventsToNitroCalendar(nitroCalendar, todayEvents);
				AddEventsToNitroCalendar(nitroCalendar, futureEvents);
			}
			catch (Exception e)
			{
				new UIAlertView("add events process", e.Message, null, "ok", null).Show();
			}
		}

		private async void AddEventsToNitroCalendar(EKCalendar nitroCalendar, string eventsJson)
		{
			if (eventsJson == null || eventsJson == "" || eventsJson == "[]")
				return;

			//eventsJson = FinterHTMLTag(eventsJson);
			var eventsData = JArray.Parse(FormatArrayType(eventsJson));

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

				string eventDescription = FilterHTMLTag(eventData["eventData"].ToString());

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
				var encodedEventURL = "http://go-heja.com/nitro/calenPage.php?name=" + encodedTitle + "&startdate=" + encodedDate + "&user=" + username;

				newEvent.Url = new NSUrl(System.Web.HttpUtility.UrlEncode(encodedEventURL)); ;

				//add alarm to event
				EKAlarm[] alarmsArray = new EKAlarm[2];
				alarmsArray[0] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 45)));
				alarmsArray[1] = EKAlarm.FromDate(newEvent.StartDate.AddSeconds(-(60 * 60 * 12)));
				newEvent.Alarms = alarmsArray;

				newEvent.Calendar = nitroCalendar;

				NSError e;
				App.Current.EventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out e);

				var todo5 = new Todo
				{
					actionName = "save event",
					status = e == null ? "null" : "error"
				};
				//track source saving status of "Nitro event" to firebase
				await _client.PushAsync("users/" + username, todo5);
			}
		}

		public NSDate ConvertDateTimeToNSDate(DateTime date)
		{
			DateTime newDate = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));

			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - newDate).TotalSeconds + 3600);
		}
		private string FormatArrayType(string eventsJson)
		{
			var returnString = eventsJson.Replace("ObjectId(\"", "\"");
			returnString = returnString.Replace(" ISODate(\"", "\"");
			returnString = returnString.Replace("\")", "\"");

			return returnString;
		}
		private string FilterHTMLTag(string eventJson)
		{
			var returnString = eventJson.Replace("<textarea id =\"genData\" class=\"generalData\" name=\"pDesc\"  placeholder=\"Right here coach\" maxlength=\"1000\">", "");
			returnString = returnString.Replace("<textarea  dir=\"rtl\" lang=\"ar\"id =\"genData\" class=\"pGenData\" name=\"pDesc\"  placeholder=\"Practice details\" maxlength=\"1000\">", "");
			returnString = returnString.Replace("</textarea><br/>", "");

			return returnString;
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


