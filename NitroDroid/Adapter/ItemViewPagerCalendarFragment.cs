
using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using MonoDroid.TimesSquare;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "ItemViewPagerCalendarFragment")]
	public class ItemViewPagerCalendarFragment : Android.Support.V4.App.Fragment
	{
		public CalendarPickerView calendar;

		Action<List<NitroEvent>> _callback;
		List<DateTime> ListDateEvent = new List<DateTime>();
		public List<NitroEvent> _events = new List<NitroEvent>();

		public ItemViewPagerCalendarFragment(Action<List<NitroEvent>> callback, List<NitroEvent> events)
		{
			_callback = callback;
			_events = events;
		}

		#region Overrides

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.ItemViewPagerCalendar, container, false);

			calendar = view.FindViewById<CalendarPickerView>(Resource.Id.calendaritem);

			_events = AppSettings.currentEventsList;
			if (_events != null && _events.Count != 0)
			{
				ListDateEvent.Clear();
				for (int i = 0; i < _events.Count; i++)
				{
					var startDate = _events[i].StartDateTime();//DateTime.Parse(_events[i].start);
					if (ListDateEvent.Count != 0)
					{
						var IsExits = false;
						for (int j = 0; j < ListDateEvent.Count; j++)
						{
							if (ListDateEvent[j].DayOfYear == startDate.DayOfYear)
								IsExits = true;
						}
						if (!IsExits)
							ListDateEvent.Add(startDate);
					}
					else
					{
						ListDateEvent.Add(startDate);
					}
				}
			}
			else
			{
				if (ListDateEvent.Count != 0)
					ListDateEvent.Clear();
			}

			FluentInitializer fInitializer = calendar.Init(DateTime.UtcNow.AddYears(-2), DateTime.UtcNow.AddYears(2))
			.InMode(CalendarPickerView.SelectionMode.Single)
			.WithHighlightedDates(ListDateEvent);

			fInitializer.WithSelectedDate(DateTime.Now.ToUniversalTime());

			calendar.OnDateSelected += (s, e) => { UpdateEventFilterByDay(e.SelectedDate); };

			UpdateEventFilterByDay(DateTime.Now);

			return view;
		}

		public override void OnResume()
		{
			base.OnResume();
		}

		#endregion

		#region Method

		#region UpdateEventFilterByDay

		public void UpdateEventFilterByDay(DateTime selectedDate)
		{
			List<NitroEvent> returnEvents = new List<NitroEvent>();

			if (_events != null && _events.Count != 0)
			{
				for (int i = 0; i < _events.Count; i++)
				{
					var startDate = _events[i].StartDateTime();
					if (startDate.DayOfYear == selectedDate.DayOfYear)
					{
						returnEvents.Add(_events[i]);
					}
				}
			}

			_callback(returnEvents);
		}


		#endregion

		#endregion
	}
}
