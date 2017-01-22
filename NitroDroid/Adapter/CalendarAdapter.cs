﻿using System;
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

		public CalendarAdapter(FragmentManager fm, Action<List<NitroEvent>> callback, List<NitroEvent> events) : base(fm)
		{
			_callback = callback;
			_events = events;
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
			Fragment fragment = new ItemViewPagerCalendarFragment(_callback, _events);
			return fragment;
		}

		public void GotoToday()
		{

		}
	}
}
