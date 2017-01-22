
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "EventCalendarActivity")]
	public class EventCalendarActivity : BaseActivity
	{
		ViewPager calendarPager;
		ListView eventsList;
		List<NitroEvent> _events = new List<NitroEvent>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EventCalendarActivity);

			ReloadEvents();

			FindViewById(Resource.Id.ActionReload).Click += (sender, e) => ReloadEvents();
			FindViewById(Resource.Id.ActionToday).Click += (sender, e) => GotoToday();
		}

		void ReloadEvents()
		{
			_events = new List<NitroEvent>();
			calendarPager = FindViewById<ViewPager>(Resource.Id.viewPager);
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Retreving Nitro Events...");

				var pastEvents = GetPastEvents();
				var todayEvents = GetTodayEvents();
				var futureEvents = GetFutureEvents();

				_events.AddRange(pastEvents);
				_events.AddRange(todayEvents);
				_events.AddRange(futureEvents);

				RunOnUiThread(() =>
				{
					calendarPager.Adapter = new CalendarAdapter(SupportFragmentManager, FilteredEventsByDate, _events);
				});

				HideLoadingView();
			});
		}

		void GotoToday()
		{
			//AppSettings.fInitializer.WithSelectedDate(DateTime.Now);
			calendarPager.Adapter = new CalendarAdapter(SupportFragmentManager, FilteredEventsByDate, _events);
		}

		void FilteredEventsByDate(List<NitroEvent> events)
		{
			eventsList = FindViewById(Resource.Id.eventsList) as ListView;
			var adapter = new NitroEventAdapter(events, this);
			eventsList.Adapter = adapter;
			adapter.NotifyDataSetChanged();
		}
	}
}
