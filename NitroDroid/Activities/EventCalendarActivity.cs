﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "EventCalendarActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class EventCalendarActivity : BaseActivity
	{
		//static bool isFirst = true;

		ViewPager calendarPager;
		ListView eventsList;
		List<NitroEvent> _events = new List<NitroEvent>();
		LinearLayout noEventsContent;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EventCalendarActivity);

			noEventsContent = FindViewById<LinearLayout>(Resource.Id.noEventsContent);
			noEventsContent.Visibility = ViewStates.Gone;

			//if (isFirst)
			//{
				ReloadEvents();
			//	isFirst = false;
			//}

			FindViewById(Resource.Id.ActionReload).Click += (sender, e) => ReloadEvents();
			FindViewById(Resource.Id.ActionToday).Click += (sender, e) => GotoToday();
		}

		void ReloadEvents()
		{
			if (!IsNetEnable()) return;

			_events = new List<NitroEvent>();
			calendarPager = FindViewById<ViewPager>(Resource.Id.viewPager);
			//calendarPager.Adapter = new CalendarAdapter(SupportFragmentManager, FilteredEventsByDate, null);

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Retreving Nitro Events...");

				var pastEvents = GetPastEvents();
				var todayEvents = GetTodayEvents();
				var futureEvents = GetFutureEvents();

				_events.AddRange(pastEvents);
				_events.AddRange(todayEvents);
				_events.AddRange(futureEvents);

				AppSettings.currentEventsList = _events;

				RunOnUiThread(() =>
				{
					calendarPager.Adapter = null;
					calendarPager.Adapter = new CalendarAdapter(SupportFragmentManager, FilteredEventsByDate, _events);
					HideLoadingView();
				});

			});
		}

		void GotoToday()
		{
			if (!IsNetEnable()) return;

			calendarPager.Adapter = null;
			calendarPager.Adapter = new CalendarAdapter(SupportFragmentManager, FilteredEventsByDate, _events);
		}

		void FilteredEventsByDate(List<NitroEvent> events)
		{
			if (events.Count == 0)
				noEventsContent.Visibility = ViewStates.Visible;
			else
				noEventsContent.Visibility = ViewStates.Gone;
			
			eventsList = FindViewById(Resource.Id.eventsList) as ListView;
			var adapter = new NitroEventAdapter(events, this);
			eventsList.Adapter = adapter;
			adapter.NotifyDataSetChanged();
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
