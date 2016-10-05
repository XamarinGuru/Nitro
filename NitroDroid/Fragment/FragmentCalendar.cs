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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Java.Util;

//by Afroz date 31/8/2016
namespace goheja
{
    public class FragmentCalendar : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.calendar, container, false);
            var webView = view.FindViewById<WebView>(Resource.Id.webViewCalen);
           
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.AllowContentAccess = true;
            webView.Settings.EnableSmoothTransition();
            webView.Settings.LoadsImagesAutomatically = true;
            webView.Settings.SetGeolocationEnabled(true);
            webView.SetWebViewClient(new WebViewClient());

            var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
            string nickName = contextPref.GetString("storedUserName", "");
            webView.LoadUrl("http://go-heja.com/nitro/mobda.php?userNickName=" + nickName + "&userId=" + contextPref.GetString("storedAthId", ""));

			RegisterEvents();
            return view;
        }

		private void RegisterEvents()
		{
			var calendarsUri = CalendarContract.Calendars.ContentUri;

			string[] calendarsProjection = {
			   CalendarContract.Calendars.InterfaceConsts.Id,
			   CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
			   CalendarContract.Calendars.InterfaceConsts.AccountName
			};

			var cursor = this.Activity.ManagedQuery(calendarsUri, calendarsProjection, null, null, null);

			string[] sourceColumns = {CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
				CalendarContract.Calendars.InterfaceConsts.AccountName};

			long calID = -1;
			if (cursor.MoveToFirst())
			{
				do
				{
					long id = cursor.GetLong(0);
					String displayName = cursor.GetString(1);
					if (displayName == "GrokkingAndroid Calendar")
						calID = id;
				} while (cursor.MoveToNext());
			}

			if (calID != -1)
			{
				Android.Net.Uri.Builder builder1 =
				   CalendarContract.Calendars.ContentUri.BuildUpon();
				builder1.AppendQueryParameter(
					CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
					  "GrokkingAndroid Calendar");
				//builder1.AppendQueryParameter(
				//	  CalendarContract.Calendars.InterfaceConsts.AccountType,
				//	CalendarContract.AccountTypeLocal);
				//builder1.AppendQueryParameter(
				//	CalendarContract.CallerIsSyncadapter,
				//	  "true");

				String[] selArgs = new String[] { "GrokkingAndroid Calendar" };
				int deleted = this.Activity.ContentResolver.Delete(CalendarContract.Calendars.ContentUri,
				                                                   CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName + " =? ",
										   selArgs);
				//var uri1 = this.Activity.ContentResolver.Delete((builder1.Build(), values);
			}
			ContentValues values = new ContentValues();
			values.Put(
				CalendarContract.Calendars.InterfaceConsts.AccountName,
				  "Nitro Events");
			values.Put(
				CalendarContract.Calendars.InterfaceConsts.AccountType,
				CalendarContract.AccountTypeLocal);
			//values.Put(
			//	CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
			//	  "GrokkingAndroid Calendar");
			values.Put(
				  CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
				  "Nitro Calendar");
			values.Put(
				CalendarContract.Calendars.InterfaceConsts.CalendarColor,
				  0xffff0000);
			//values.Put(
			//	CalendarContract.Calendars.InterfaceConsts.CalendarAccessLevel,
			//	Android.Provider.CalendarAccess.AccessOwner);
			//CalendarContract.Calendars.InterfaceConsts.CalAccessOwner);
			values.Put(
				CalendarContract.Calendars.InterfaceConsts.OwnerAccount,
				  "erlend0720@hotmail.com");
			values.Put(
				CalendarContract.Calendars.InterfaceConsts.CalendarTimeZone,
				  "Europe/Berlin");
			values.Put(
				CalendarContract.Calendars.InterfaceConsts.SyncEvents,
				  1);
			Android.Net.Uri.Builder builder =
				   CalendarContract.Calendars.ContentUri.BuildUpon();
			builder.AppendQueryParameter(
				CalendarContract.Calendars.InterfaceConsts.AccountName,
				  "com.grokkingandroid");
			builder.AppendQueryParameter(
				  CalendarContract.Calendars.InterfaceConsts.AccountType,
				CalendarContract.AccountTypeLocal);
			builder.AppendQueryParameter(
				CalendarContract.CallerIsSyncadapter,
				  "true");
			var uri = this.Activity.ContentResolver.Insert(builder.Build(), values);
			//for (var i = 0; i < cursor.Count; i++)
			//{
			//	cursor.MoveToPosition(i);
			//	var calName = cursor.GetString(cursor.GetColumnIndex(calendarsProjection[1]));

			//}
			//int[] targetResources = { Resource.Id.calDisplayName, Resource.Id.calAccountName };

			//SimpleCursorAdapter adapter = new SimpleCursorAdapter(this, Resource.Layout.CalListItem,
			//	cursor, sourceColumns, targetResources);

			//ListAdapter = adapter;

			//ListView.ItemClick += (sender, e) =>
			//{
			//	int i = (e as Android.Widget.AdapterView.ItemClickEventArgs).Position;

			//	cursor.MoveToPosition(i);
			//	int calId = cursor.GetInt(cursor.GetColumnIndex(calendarsProjection[0]));

			//	var showEvents = new Intent(this, typeof(EventListActivity));
			//	showEvents.PutExtra("calId", calId);
			//	StartActivity(showEvents);
			//};

			ContentValues eventValues = new ContentValues();
			eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarDisplayName, "Nitro Events");
			
			eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, "1");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Title, "Test Event from M4A");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Description, "This is an event created from Mono for Android");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS(2016, 10, 2, 10, 0));
			eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS(2016, 10, 2, 11, 0));

			// GitHub issue #9 : Event start and end times need timezone support.
			// https://github.com/xamarin/monodroid-samples/issues/9
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
			eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");

			var uri2 = this.Activity.ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
			Console.WriteLine("Uri for new event: {0}", uri);
		}

		long GetDateTimeMS(int yr, int month, int day, int hr, int min)
		{
			Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

			c.Set(Java.Util.CalendarField.DayOfMonth, day);
			c.Set(Java.Util.CalendarField.HourOfDay, hr);
			c.Set(Java.Util.CalendarField.Minute, min);
			c.Set(Java.Util.CalendarField.Month, month);
			c.Set(Java.Util.CalendarField.Year, yr);

			return c.TimeInMillis;
		}
    }
}
//end by Afroz date 31/8/2016