using System;
using System.Collections.Generic;
using Android.Support.V4.App;
using PortableLibrary;

namespace goheja
{
	public class CalendarAdapter : FragmentPagerAdapter
	{
		private int MaxCount = 200;
		private Action<List<NitroEvent>> _callback;
		private List<NitroEvent> _events;

		public static ItemViewPagerCalendarFragment fragment;

		public CalendarAdapter(FragmentManager fm, Action<List<NitroEvent>> callback, List<NitroEvent> events) : base(fm)
		{
			_callback = callback;
			_events = events;
			AppSettings.currentEventsList = events;
			//if (fragment!= null)
			//	AppSettings.currentEventsList = new List<NitroEvent>();
		}

		public override int Count
		{
			get
			{
				return MaxCount;
			}
		}

		public override Fragment GetItem(int position)
		{
			fragment = new ItemViewPagerCalendarFragment(_callback, _events);
			return fragment;
		}

		public void GotoToday()
		{

		}
	}
}
