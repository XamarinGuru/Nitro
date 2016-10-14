using System;
using Android.App;
using Android.Util;
using System.Threading;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Text.RegularExpressions;
using Android.Content.Res;
using Android.Preferences;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Runtime;
using Android.Views;
using Android.Webkit;

using Android.Provider;

using Java.Util;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace goheja
{
	[Service]
	public class BackgroundService : Service
	{
		private string userName;
		System.Threading.Timer _timer;

		[Android.Runtime.Register("onStart", "(Landroid/content/Intent;I)V", "GetOnStart_Landroid_content_Intent_IHandler")]
		[System.Obsolete("deprecated")]

		public override void OnStart(Android.Content.Intent intent, int startId)
		{
			base.OnStart(intent, startId);

			DoStuff();
		}

		public override void OnDestroy()
		{
			base.OnDestroy();

			_timer.Dispose();

			Log.Debug("SimpleService", "SimpleService stopped");
		}

		public void DoStuff()
		{
			_timer = new System.Threading.Timer((o) =>
			{
				Log.Debug("Notifications", "hello from simple service");
				FireNotification();
			}, null, 0, 1000 * 60 * 30);
		}

		public override Android.OS.IBinder OnBind(Android.Content.Intent intent)
		{
			throw new NotImplementedException();
		}

		public void FireNotification()
		{
			trackSvc.Service1 meServ = new trackSvc.Service1();
			meServ = new trackSvc.Service1();

			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			userName = contextPref.GetString("storedUserName", "");
			//userName = "Inna";

			var pastEvents = "";// meServ.getUserCalendarPast(userName);
			var todayEvents = meServ.getUserCalendarToday(userName);
			var futureEvents = meServ.getUserCalendarFuture(userName);

			AddEvents(pastEvents, todayEvents, futureEvents);

		}
		private void AddEvents(string pastEvents, string todayEvents, string futureEvents)
		{
			var calendarsUri = CalendarContract.Calendars.ContentUri;

			string[] calendarsProjection = {
			   CalendarContract.Calendars.InterfaceConsts.Id,
			   CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
			   CalendarContract.Calendars.InterfaceConsts.AccountName
			};

			var cursor = ApplicationContext.ContentResolver.Query(calendarsUri, calendarsProjection, null, null, null);

			long calID = -1;
			#region remove existing Nitro calendar
			if (cursor.MoveToFirst())
			{
				do
				{
					long id = cursor.GetLong(0);
					String displayName = cursor.GetString(1);
					if (displayName == "Nitro Calendar")
						//RemoveCalendar(id);
						calID = id;
				} while (cursor.MoveToNext());
			}
			#endregion

			#region create Nitro Calendar
			if (calID == -1)
			{
				var uri = CalendarContract.Calendars.ContentUri;
				ContentValues val = new ContentValues();
				val.Put(CalendarContract.Calendars.InterfaceConsts.AccountName, "Nitro Calendar");
				val.Put(CalendarContract.Calendars.InterfaceConsts.AccountType, CalendarContract.AccountTypeLocal);
				val.Put(CalendarContract.Calendars.Name, "Nitro Calendar");
				val.Put(CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName, "Nitro Calendar");
				val.Put(CalendarContract.Calendars.InterfaceConsts.CalendarColor, Android.Graphics.Color.Black);
				val.Put(CalendarContract.Calendars.InterfaceConsts.OwnerAccount, "Nitro Calendar");
				val.Put(CalendarContract.Calendars.InterfaceConsts.Visible, true);
				val.Put(CalendarContract.Calendars.InterfaceConsts.SyncEvents, true);
				uri = uri.BuildUpon()
					.AppendQueryParameter(CalendarContract.CallerIsSyncadapter, "true")
					.AppendQueryParameter(CalendarContract.Calendars.InterfaceConsts.AccountName, "Nitro Calendar")
					.AppendQueryParameter(CalendarContract.Calendars.InterfaceConsts.AccountType, CalendarContract.AccountTypeLocal)
					.Build();
				var calresult = ContentResolver.Insert(uri, val);
				calID = long.Parse(calresult.LastPathSegment);
			}

			#endregion
			RemoveNitroTodayAndFutureEvents(calID);

			//AddEventsToNitroCalendar(calID, pastEvents);
			AddEventsToNitroCalendar(calID, todayEvents);
			AddEventsToNitroCalendar(calID, futureEvents);
		}

		private void RemoveNitroTodayAndFutureEvents(long calID)
		{
			var eventURI = CalendarContract.Events.ContentUri;

			string[] eventsProjection = {
				//CalendarContract.Calendars.InterfaceConsts.Id,
				//CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
				//CalendarContract.Calendars.InterfaceConsts.AccountName
				"_id",
			    CalendarContract.Events.InterfaceConsts.Title
			   , CalendarContract.Events.InterfaceConsts.Dtstart
			   , CalendarContract.Events.InterfaceConsts.Dtend
			};
			DateTime now = DateTime.Now;
			DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
			var startString = GetDateTimeMS(startNow).ToString();
			var endString = GetDateTimeMS(DateTime.Now.AddYears(5)).ToString();
			var selection = "((dtstart >= ?) AND (dtend <= ?) AND (calendar_id = ?))";
			var selectionArgs = new string[] { startString, endString, calID.ToString() };
			var calCursor = ApplicationContext.ContentResolver.Query(eventURI, eventsProjection, selection, selectionArgs, null);
			//var calCursor = ApplicationContext.ContentResolver.Query(eventURI, eventsProjection, null, null, null);
			var cou = calCursor.Count;
			if (calCursor.MoveToFirst())
			{
				do
				{
					long id = calCursor.GetLong(0);
					String displayName = calCursor.GetString(1);
					long eventId = calCursor.GetLong(calCursor.GetColumnIndex("_id"));
					RemoveEvent(eventId);
					// …
				} while (calCursor.MoveToNext());
			}
		}

		private void AddEventsToNitroCalendar(long calID, string eventsJson)
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

				var startDate = Convert.ToDateTime(eventData["start"].ToString());//DateTime.Parse(eventData["start"].ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);//Convert.ToDateTime(eventData["start"].ToString());
				var endDate = Convert.ToDateTime(eventData["end"].ToString());

				//var deltaSec = (startDate - DateTime.Now).TotalSeconds;
				//if (deltaSec < 0)
				//	continue;

				var title = eventData["title"].ToString();

				string eventDescription = eventData["eventData"].ToString();

				var filteredDescription = RemoveHTMLTag(eventDescription);

				string[] arryEventDes = filteredDescription.Split(new char[] { '~' });

				string note = "";
				for (var i = 0; i < arryEventDes.Length; i++)
				{
					note += arryEventDes[i].ToString() + System.Environment.NewLine;
				}


				var strDistance = eventData["distance"].ToString();
				float floatDistance = strDistance == "" ? 0 : float.Parse(strDistance);
				var b = Math.Truncate(floatDistance * 100);
				var c = b / 100;
				var formattedDistance = c.ToString("F2");

				var durMin = eventData["durMin"].ToString() == "" ? 0 : int.Parse(eventData["durMin"].ToString());
				var durHrs = eventData["durHrs"].ToString() == "" ? 0 : int.Parse(eventData["durHrs"].ToString());
				var pHrs = durMin / 60;
				durHrs = durHrs + pHrs;
				durMin = durMin % 60;

				var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

				note += System.Environment.NewLine + "Planned HB : " + eventData["hb"].ToString() + System.Environment.NewLine +
								"Planned TSS : " + eventData["tss"].ToString() + System.Environment.NewLine +
								"Planned distance : " + formattedDistance + "KM" + System.Environment.NewLine +
								"Duration : " + strDuration + System.Environment.NewLine;

				var encodedTitle = System.Web.HttpUtility.UrlEncode(eventData["title"].ToString());

				var strDate = String.Format("{0:dd-MM-yyyy hh:mm:ss}", startDate);
				var encodedDate = System.Web.HttpUtility.UrlEncode(strDate);
				var encodedEventURL = "http://go-heja.com/nitro/calenPage.php?name=" + encodedTitle + "&startdate=" + encodedDate + "&user=" + userName;

				note += System.Environment.NewLine + encodedEventURL;

				#region create event
				ContentValues eventValues = new ContentValues();
				eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, calID);
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, title);
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, note);
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS(startDate));
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS(endDate));
				eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
				eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");
				eventValues.Put(CalendarContract.Events.InterfaceConsts.HasAlarm, 1);

				var eventURI = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
				var eventID = long.Parse(eventURI.LastPathSegment);


				var deltaMin = (startDate - DateTime.Now).TotalMinutes;
				if (deltaMin > 45)
				{
					ContentValues reminderValues1 = new ContentValues();
					reminderValues1.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
					reminderValues1.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 45.0f);
					reminderValues1.Put(CalendarContract.Reminders.InterfaceConsts.Method, 1);
					ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues1);

					if (deltaMin > (10 * 60))
					{
						ContentValues reminderValues2 = new ContentValues();
						reminderValues2.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
						reminderValues2.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 60 * 10);
						reminderValues2.Put(CalendarContract.Reminders.InterfaceConsts.Method, 1);
						ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues2);
					}
				}

				#endregion
			}


		}

		private void RemoveCalendar(long calID)
		{
			Android.Net.Uri.Builder builder1 = CalendarContract.Calendars.ContentUri.BuildUpon();
			builder1.AppendQueryParameter(CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName, "Nitro Calendar");

			String[] selArgs = new String[] { "Nitro Calendar" };
			int deleted = ContentResolver.Delete(CalendarContract.Calendars.ContentUri, CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName + " =? ", selArgs);
		}
		private void RemoveEvent(long eventID)
		{
			String[] selArgs = new String[] { eventID.ToString() };
			int deleted = ContentResolver.Delete(CalendarContract.Events.ContentUri, "_id =? ", selArgs);
		}
		long GetDateTimeMS(DateTime date)
		{
			Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);
			c.Set(Java.Util.CalendarField.DayOfMonth, date.Day);
			c.Set(Java.Util.CalendarField.HourOfDay, date.Hour);
			c.Set(Java.Util.CalendarField.Minute, date.Minute);
			c.Set(Java.Util.CalendarField.Month, (date.Month - 1));
			c.Set(Java.Util.CalendarField.Year, date.Year);

			return c.TimeInMillis;
		}

		private string RemoveHTMLTag(string rawString)
		{
			if (rawString == "") return rawString;

			int startIndex = rawString.IndexOf("<textarea");
			int endIndex = rawString.IndexOf(">");
			int count = endIndex - startIndex;

			var theString = new StringBuilder(rawString);
			theString.Remove(startIndex, count);

			var returnString = theString.ToString();
			returnString = returnString.Replace("</textarea><br/>", "");
			return returnString;
		}
	}
}