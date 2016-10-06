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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;

using Android.Provider;

using Java.Util;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace goheja
{
	[Service]
	public class BackgroundService : Service
	{
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
			}, null, 0, 1000 * 3600);
		}

		public override Android.OS.IBinder OnBind(Android.Content.Intent intent)
		{
			throw new NotImplementedException();
		}

		public void FireNotification()
		{
			trackSvc.Service1 meServ = new trackSvc.Service1();
			meServ = new trackSvc.Service1();

			//var nickName = "efrendsen";
			var pastEvents = meServ.getUserCalendarPast(nickName);
			var todayEvents = meServ.getUserCalendarToday(nickName);
			var futureEvents = meServ.getUserCalendarFuture(nickName);

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

			#region remove existing Nitro calendar
			if (cursor.MoveToFirst())
			{
				do
				{
					long id = cursor.GetLong(0);
					String displayName = cursor.GetString(1);
					if (displayName == "Nitro Calendar")
						RemoveCalendar(id);
				} while (cursor.MoveToNext());
			}
			#endregion

			#region create Nitro Calendar
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
			var calID = long.Parse(calresult.LastPathSegment);
			#endregion

			AddEventsToNitroCalendar(calID, pastEvents);
			AddEventsToNitroCalendar(calID, todayEvents);
			AddEventsToNitroCalendar(calID, futureEvents);
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

				var startDate = DateTime.Parse(eventData["start"].ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind);//Convert.ToDateTime(eventData["start"].ToString());
				var endDate = Convert.ToDateTime(eventData["end"].ToString());

				var title = eventData["title"].ToString();

				string eventDescription = eventData["eventData"].ToString();
				eventDescription = eventDescription.Replace("<textarea id =\"genData\" class=\"generalData\" name=\"pDesc\"  placeholder=\"Right here coach\" maxlength=\"1000\">", "");
				eventDescription = eventDescription.Replace("</textarea><br/>", "");

				string[] arryEventDes = eventDescription.Split(new char[] { '~' });

				string note = "";
				for (var i = 0; i < arryEventDes.Length; i++)
				{
					note += arryEventDes[i].ToString() + System.Environment.NewLine;
				}

				var strDistance = eventData["distance"].ToString();
				var floatDistance = float.Parse(strDistance);
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

				//var structuredLocation = new EKStructuredLocation();
				//structuredLocation.Title = "my location";
				//structuredLocation.GeoLocation = new CoreLocation.CLLocation(100, 100);

				#region create event
				ContentValues eventValues = new ContentValues();
				eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, calID);
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, title);
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, note);
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS(startDate));
				eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS(endDate));
				eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
				eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");

				var eventURI = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
				var eventID = long.Parse(eventURI.LastPathSegment);

				ContentValues reminderValues = new ContentValues();
				reminderValues.Clear();
				reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventID);
				reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 45);
				reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, 60 * 12);
				ContentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues);
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
	}
}