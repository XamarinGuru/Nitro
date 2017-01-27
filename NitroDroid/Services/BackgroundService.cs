using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Provider;
using Java.Util;
using Newtonsoft.Json.Linq;
using PortableLibrary;

namespace goheja
{
	[Service]
	public class BackgroundService : Service
	{
		System.Threading.Timer _timer;

		private long calendarID = -1;

		public BaseActivity baseVC;

		[Android.Runtime.Register("onStart", "(Landroid/content/Intent;I)V", "GetOnStart_Landroid_content_Intent_IHandler")]
		[System.Obsolete("deprecated")]

		public override void OnStart(Intent intent, int startId)
		{
			base.OnStart(intent, startId);

			if (AppSettings.UserID == null || AppSettings.baseVC == null)
				return;

			baseVC = AppSettings.baseVC;

			StartUpdateTimer();
		}

		public override void OnDestroy()
		{
			base.OnDestroy();

			_timer.Dispose();
		}

		public override Android.OS.IBinder OnBind(Intent intent)
		{
			throw new NotImplementedException();
		}

		public void StartUpdateTimer()
		{
			_timer = new System.Threading.Timer((o) =>
			{
				AddNitroCalendarToDevice();
			} , null, 0, 1000 * 60 * 30);
		}

		private void AddNitroCalendarToDevice()
		{
			var calendarsUri = CalendarContract.Calendars.ContentUri;

			string[] calendarsProjection = {
			   CalendarContract.Calendars.InterfaceConsts.Id,
			   CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
			   CalendarContract.Calendars.InterfaceConsts.AccountName
			} ;

			var cursor = ApplicationContext.ContentResolver.Query(calendarsUri, calendarsProjection, null, null, null);

			#region remove existing Nitro calendar
			if (cursor.MoveToFirst())
			{
				do
				{
					long id = cursor.GetLong(0);
					String displayName = cursor.GetString(1);
					if (displayName == "Nitro Calendar")
						//RemoveCalendar(id);
						calendarID = id;
				}  while (cursor.MoveToNext());
			}
			#endregion

			#region create Nitro Calendar
			if (calendarID == -1)
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
				calendarID = long.Parse(calresult.LastPathSegment);
			}

			#endregion
			RemoveNitroTodayAndFutureEvents();

			UpdateNitroEvents();
		}

		public void UpdateNitroEvents()
		{
			var pastEvents = baseVC.GetPastEvents();
			var todayEvents = baseVC.GetTodayEvents();
			var futureEvents = baseVC.GetFutureEvents();

			AddEventsToNitroCalendar(pastEvents);
			AddEventsToNitroCalendar(todayEvents);
			AddEventsToNitroCalendar(futureEvents);
		}

		private void RemoveNitroTodayAndFutureEvents()
		{
			if (calendarID == -1) return;

			var eventURI = CalendarContract.Events.ContentUri;

			string[] eventsProjection = {
				"_id",
			    CalendarContract.Events.InterfaceConsts.Title
			   , CalendarContract.Events.InterfaceConsts.Dtstart
			   , CalendarContract.Events.InterfaceConsts.Dtend
			} ;
			DateTime now = DateTime.Now;
			DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
			var startString = GetDateTimeMS(startNow).ToString();
			var endString = GetDateTimeMS(DateTime.Now.AddYears(5)).ToString();
			var selection = "((dtstart >= ?) AND (dtend <= ?) AND (calendar_id = ?))";
			var selectionArgs = new string[] { startString, endString, calendarID.ToString() };
			var calCursor = ApplicationContext.ContentResolver.Query(eventURI, eventsProjection, selection, selectionArgs, null);
			var cou = calCursor.Count;
			if (calCursor.MoveToFirst())
			{
				do
				{
					long id = calCursor.GetLong(0);
					String displayName = calCursor.GetString(1);
					long eventId = calCursor.GetLong(calCursor.GetColumnIndex("_id"));
					RemoveEvent(eventId);
				}  while (calCursor.MoveToNext());
			}
		}

		private void AddEventsToNitroCalendar(List<NitroEvent> events)
		{
			if (calendarID == -1 || events == null || events.Count == 0) return;

			foreach (var nitroEvent in events)
			{
				var startDate = nitroEvent.StartDateTime();//Convert.ToDateTime(nitroEvent.start);
				var endDate = nitroEvent.EndDateTime();//Convert.ToDateTime(nitroEvent.end);

				DateTime now = DateTime.Now;
				DateTime startNow = new DateTime(now.Year, now.Month, now.Day);
				DateTime startDay = new DateTime(startDate.Year, startDate.Month, startDate.Day);
				var deltaSec = (startDay - startNow).TotalSeconds;
				if (deltaSec < 0)
					continue;

				var title = nitroEvent.title;

				string eventDescription = nitroEvent.eventData;

				var filteredDescription = baseVC.FormatEventDescription(eventDescription);

				string[] arryEventDes = filteredDescription.Split(new char[] { '~' });

				string note = "";
				for (var i = 0; i < arryEventDes.Length; i++)
				{
					note += arryEventDes[i] + Environment.NewLine;
				}


				var strDistance = nitroEvent.distance;
				float floatDistance = strDistance == "" ? 0 : float.Parse(strDistance);
				var b = Math.Truncate(floatDistance * 100);
				var c = b / 100;
				var formattedDistance = c.ToString("F2");

				var durMin = nitroEvent.durMin == "" ? 0 : int.Parse(nitroEvent.durMin);
				var durHrs = nitroEvent.durHrs == "" ? 0 : int.Parse(nitroEvent.durHrs);
				var pHrs = durMin / 60;
				durHrs = durHrs + pHrs;
				durMin = durMin % 60;

				var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

				note += System.Environment.NewLine + "Planned HB : " + nitroEvent.hb + Environment.NewLine +
				              "Planned TSS : " + nitroEvent.tss + Environment.NewLine +
								"Planned distance : " + formattedDistance + "KM" + Environment.NewLine +
								"Duration : " + strDuration + Environment.NewLine;

				var encodedTitle = System.Web.HttpUtility.UrlEncode(nitroEvent.title);

				var strDate = startDate.ToString();//String.Format("{ 0:MM/dd/yyyy hh:mm:ss a}", startDate.ToString());
				var encodedDate = System.Web.HttpUtility.UrlEncode(strDate);
				var encodedEventURL = String.Format(Constants.URL_EVENT_MAP, encodedTitle, encodedDate, AppSettings.Username);

				note += Environment.NewLine + encodedEventURL;

				#region create event
				ContentValues eventValues = new ContentValues();
				eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, calendarID);
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
			c.Set(CalendarField.DayOfMonth, date.Day);
			c.Set(CalendarField.HourOfDay, date.Hour);
			c.Set(CalendarField.Minute, date.Minute);
			c.Set(CalendarField.Month, (date.Month - 1));
			c.Set(CalendarField.Year, date.Year);

			return c.TimeInMillis;
		}


	}
}