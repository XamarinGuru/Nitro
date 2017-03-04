
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.GrapeCity.Xuni.Calendar;
using Com.GrapeCity.Xuni.Core;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "EventCalendarActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class EventCalendarActivity : BaseActivity
	{
		XuniCalendar calendar;

		ListView eventsList;
		List<NitroEvent> _events = new List<NitroEvent>();
		LinearLayout noEventsContent;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EventCalendarActivity);

			#region xuni calendar
			calendar = FindViewById<XuniCalendar>(Resource.Id.calendar);
			calendar.Orientation = CalendarOrientation.Vertical;
			calendar.MaxSelectionCount = 1;

			// change appearance
			calendar.DayOfWeekBackgroundColor = System.Drawing.Color.Transparent.ToArgb();
			calendar.DayOfWeekTextColor = System.Drawing.Color.LightGray.ToArgb();
			calendar.TodayTextColor = System.Drawing.Color.Red.ToArgb();
			calendar.SelectionBackgroundColor = System.Drawing.Color.Orange.ToArgb();

			calendar.DaySlotLoading += Calendar_DaySlotLoading;
			calendar.SelectionChanged += CalendarSelectionChanged;
			#endregion

			noEventsContent = FindViewById<LinearLayout>(Resource.Id.noEventsContent);
			noEventsContent.Visibility = ViewStates.Gone;

			ReloadEvents();

			FindViewById(Resource.Id.ActionReload).Click += (sender, e) => ReloadEvents();
			FindViewById(Resource.Id.ActionToday).Click += (sender, e) => GotoToday();
		}

		void CalendarSelectionChanged(object sender, CalendarSelectionChangedEventArgs e)
		{
			var filterDate = (sender as XuniCalendar).SelectedDate;
			FilterEventsByDate(filterDate);
		}

		void FilterEventsByDate(DateTime filterDate)
		{
			List<NitroEvent> filteredEvents = new List<NitroEvent>();

			if (_events != null && _events.Count != 0)
			{
				for (int i = 0; i < _events.Count; i++)
				{
					var startDate = _events[i].StartDateTime();
					if (startDate.DayOfYear == filterDate.DayOfYear)
					{
						filteredEvents.Add(_events[i]);
					}
				}
			}

			if (filteredEvents.Count == 0)
				noEventsContent.Visibility = ViewStates.Visible;
			else
				noEventsContent.Visibility = ViewStates.Gone;

			eventsList = FindViewById(Resource.Id.eventsList) as ListView;
			var adapter = new NitroEventAdapter(filteredEvents, this);
			eventsList.Adapter = adapter;
			adapter.NotifyDataSetChanged();
			SetListViewHeightBasedOnChildren(eventsList);

			InitPerformanceData(filterDate);
		}

		private void Calendar_DaySlotLoading(object sender, CalendarDaySlotLoadingEventArgs e)
		{
			// get day
			var currentDateTime = FromUnixTime(e.Date.Time);

			Java.Util.Date date = e.Date;
			Java.Util.Calendar cal = Java.Util.Calendar.GetInstance(Java.Util.Locale.English);
			cal.Time = date;
			int day = cal.Get(Java.Util.CalendarField.DayOfMonth);

			CalendarDaySlotBase layout = new CalendarDaySlotBase(ApplicationContext);
			layout.SetGravity(GravityFlags.Center);
			layout.SetVerticalGravity(GravityFlags.Center);
			layout.Orientation = Orientation.Vertical;
			layout.SetPadding(5, 5, 5, 5);
			LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			layout.LayoutParameters = linearLayoutParams;

			TextView tv = new TextView(ApplicationContext);
			tv.Gravity = GravityFlags.Center;
			tv.Text = day.ToString();

			if (currentDateTime.DayOfYear == DateTime.Now.DayOfYear)
				tv.SetTextColor(Android.Graphics.Color.Red);

			if (e.AdjacentDay)
				tv.SetTextColor(Android.Graphics.Color.DarkGray);

			layout.AddView(tv);

			if (_events != null && _events.Count != 0)
			{
				for (int i = 0; i < _events.Count; i++)
				{
					var startDate = _events[i].StartDateTime();
					if (startDate.DayOfYear == currentDateTime.DayOfYear)
					{
						tv.SetBackgroundColor(Android.Graphics.Color.Orange);
					}
				}
			}

			e.DaySlot = layout;
		}


		void ReloadEvents()
		{
			if (!IsNetEnable()) return;

			_events = new List<NitroEvent>();

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENTS);

				var pastEvents = GetPastEvents();
				var todayEvents = GetTodayEvents();
				var futureEvents = GetFutureEvents();

				_events.AddRange(pastEvents);
				_events.AddRange(todayEvents);
				_events.AddRange(futureEvents);

				AppSettings.currentEventsList = _events;

				RunOnUiThread(() =>
				{
					calendar.Refresh();
					FilterEventsByDate(DateTime.Now);
					calendar.SelectedDate = DateTime.Now;

					HideLoadingView();
				});
			});
		}

		void GotoToday()
		{
			if (!IsNetEnable()) return;

			calendar.ChangeViewMode(CalendarViewMode.Month, new Java.Util.Date(DateTime.Now.ToString()));
		}



		void InitPerformanceData(DateTime date)
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENTS);

				var performanceData = GetPerformanceForDate(date);

				HideLoadingView();

				if (performanceData == null) return;

				RunOnUiThread(() =>
				{
					FindViewById<TextView>(Resource.Id.lblTSB).Text = performanceData.TSB == "NaN" ? "0" : performanceData.TSB;
					FindViewById<TextView>(Resource.Id.lblCTL).Text = performanceData.CTL == "NaN" ? "0" : performanceData.CTL;
					FindViewById<TextView>(Resource.Id.lblATL).Text = performanceData.ATL == "NaN" ? "0" : performanceData.ATL;
					FindViewById<TextView>(Resource.Id.lblLoad).Text = performanceData.LOAD == "NaN" ? "0" : performanceData.LOAD;

				});
			});

		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				var activity = new Intent(this, typeof(SwipeTabActivity));
				StartActivity(activity);
				Finish();
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
