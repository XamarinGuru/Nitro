
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
		ListView eventsList;
		List<NitroEvent> _events = new List<NitroEvent>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EventCalendarActivity);

			var viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
			//viewPager.Adapter = new CalendarAdapter(SupportFragmentManager);
			

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
					viewPager.Adapter = new CalendarAdapter(SupportFragmentManager, FilteredEventsByDate, _events);
				});

				HideLoadingView();
			});
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
